using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase principal que representa una poblaci�n de conejos
public class Population : MonoBehaviour
{
    // Tama�o de la poblaci�n inicial de conejos
    public int populationSize = 100;

    // Objeto del entorno que contiene a los conejos
    public GameObject environment;

    // Lista de conejos en la poblaci�n
    protected List<Bunny> population = new List<Bunny>();

    // M�todo de inicializaci�n
    void Start()
    {
        // Obtiene los l�mites del entorno para ubicar a los conejos dentro de �l
        Bounds boundaries = environment.GetComponent<Renderer>().bounds;

        // Crea la poblaci�n inicial de conejos
        for (int i = 0; i < populationSize; i++)
        {
            Bunny bunny = CreateBunny(boundaries);
            population.Add(bunny);
        }

        // Inicia el bucle de evaluaci�n de la poblaci�n
        StartCoroutine(EvaluationLoop());
    }

    // M�todo para crear un conejo en una posici�n aleatoria dentro de los l�mites del entorno
    public Bunny CreateBunny(Bounds bounds)
    {
        // Genera una posici�n aleatoria dentro del entorno
        Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f) * bounds.size.x,
                                             UnityEngine.Random.Range(-0.5f, 0.5f) * bounds.size.y,
                                             UnityEngine.Random.Range(-0.5f, 0.5f) * bounds.size.z);

        // Calcula la posici�n mundial sumando la posici�n del entorno a la posici�n aleatoria
        Vector3 worldPosition = environment.transform.position + randomPosition;

        // Crea un objeto c�psula para representar al conejo y lo asocia con la clase Bunny
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        Bunny bunny = temp.AddComponent<Bunny>();

        // Ajusta la posici�n en el eje y para que el conejo se asiente en el suelo
        float height = temp.GetComponent<MeshFilter>().mesh.bounds.size.y;
        worldPosition.y += height / 2.0f;

        // Asigna la posici�n y un color aleatorio al conejo
        temp.transform.position = worldPosition;
        AssingRandomColor(temp);

        return bunny;
    }

    // Asigna un color aleatorio al conejo dado
    public void AssingRandomColor(GameObject bunny)
    {
        bunny.GetComponent<Bunny>().SetColor(new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)));
    }

    // Eval�a la aptitud de un conejo comparando su color con el color del entorno
    float EvaluateFitness(Color environment, Color bunny)
    {
        float fitness = (new Vector3(environment.r, environment.g, environment.b) - new Vector3(bunny.r, bunny.g, bunny.b)).magnitude;
        return fitness;
    }

    // Eval�a toda la poblaci�n de conejos
    void EvaluatePopulation()
    {
        // Calcula el fitness de cada conejo en funci�n de su color y el del entorno
        for (int i = 0; i < population.Count; i++)
        {
            float fitness = EvaluateFitness(environment.GetComponent<MeshRenderer>().material.color, population[i].GetComponent<MeshRenderer>().material.color);
            population[i].fitnessScore = fitness;
        }

        // Ordena la poblaci�n seg�n la puntuaci�n de fitness, de menor a mayor
        population.Sort(
            delegate (Bunny bunny1, Bunny bunny2)
            {
                if (bunny1.fitnessScore > bunny2.fitnessScore)
                    return 1;
                else if (bunny1.fitnessScore == bunny2.fitnessScore)
                    return 0;
                else
                    return -1;
            });

        // Elimina la mitad menos apta de la poblaci�n
        int halfwayMark = (int)population.Count / 2;
        if (halfwayMark % 2 != 0)
            halfwayMark++;
        for (int i = halfwayMark; i < population.Count; i++)
        {
            Destroy(population[i].gameObject);
            population[i] = null;
        }

        // Elimina los elementos nulos de la lista de poblaci�n
        population.RemoveRange(halfwayMark, population.Count - halfwayMark);

        // Realiza la reproducci�n para crear nuevos conejos
        Breed();
    }

    // M�todo para reproducir conejos y generar nuevos individuos
    void Breed()
    {
        // Lista temporal para almacenar los nuevos conejos
        List<Bunny> tempList = new List<Bunny>();

        // Itera en pares para cruzar los genes de los conejos
        for (int i = 1; i < population.Count; i += 2)
        {
            int breeder1Index = i - 1;
            int breeder2Index = i;

            // Determina c�mo se mezclan los genes de los padres
            float howGenesAreSplit = UnityEngine.Random.Range(0.0f, 1.0f);

            Bounds bounds = environment.GetComponent<Renderer>().bounds;

            // Crea dos nuevos conejos hijos en el entorno
            Bunny childBunny1 = CreateBunny(bounds);
            Bunny childBunny2 = CreateBunny(bounds);

            tempList.Add(childBunny1);
            tempList.Add(childBunny2);

            // Mezcla los genes de color de los padres seg�n el valor aleatorio howGenesAreSplit y aplica mutaciones
            if (howGenesAreSplit <= 0.16f)
            {
                Color tempColor = new Color(population[breeder1Index].color.r, population[breeder1Index].color.g, population[breeder2Index].color.b);
                tempColor = EvaluateMutation(tempColor);
                childBunny1.SetColor(tempColor);

                tempColor = new Color(population[breeder2Index].color.r, population[breeder1Index].color.g, population[breeder2Index].color.b);
                tempColor = EvaluateMutation(tempColor);
                childBunny2.SetColor(tempColor);
            }
            else if (howGenesAreSplit <= 0.32f)
            {
                Color tempColor = new Color(population[breeder1Index].color.r, population[breeder1Index].color.g, population[breeder1Index].color.b);
                tempColor = EvaluateMutation(tempColor);
                childBunny1.SetColor(tempColor);

                tempColor = new Color(population[breeder2Index].color.r, population[breeder1Index].color.g, population[breeder2Index].color.b);
                tempColor = EvaluateMutation(tempColor);
                childBunny2.SetColor(tempColor);
            }
            else if (howGenesAreSplit <= 0.48f)
            {
                Color tempColor = new Color(population[breeder1Index].color.r, population[breeder2Index].color.g, population[breeder2Index].color.b);
                tempColor = EvaluateMutation(tempColor);
                childBunny1.SetColor(tempColor);

                tempColor = new Color(population[breeder2Index].color.r, population[breeder1Index].color.g, population[breeder1Index].color.b);
                tempColor = EvaluateMutation(tempColor);
                childBunny2.SetColor(tempColor);
            }
            else if (howGenesAreSplit <= 0.64f)
            {
                Color tempColor = new Color(population[breeder2Index].color.r, population[breeder1Index].color.g, population[breeder1Index].color.b);
                tempColor = EvaluateMutation(tempColor);
                childBunny1.SetColor(tempColor);

                tempColor = new Color(population[breeder1Index].color.r, population[breeder2Index].color.g, population[breeder2Index].color.b);
                tempColor = EvaluateMutation(tempColor);
                childBunny2.SetColor(tempColor);
            }
            else if (howGenesAreSplit <= 0.80f)
            {
                Color tempColor = new Color(population[breeder2Index].color.r, population[breeder2Index].color.g, population[breeder1Index].color.b);
                tempColor = EvaluateMutation(tempColor);
                childBunny1.SetColor(tempColor);

                tempColor = new Color(population[breeder1Index].color.r, population[breeder1Index].color.g, population[breeder2Index].color.b);
                tempColor = EvaluateMutation(tempColor);
                childBunny2.SetColor(tempColor);
            }
            else
            {
                Color tempColor = new Color(population[breeder2Index].color.r, population[breeder1Index].color.g, population[breeder2Index].color.b);
                tempColor = EvaluateMutation(tempColor);
                childBunny1.SetColor(tempColor);

                tempColor = new Color(population[breeder1Index].color.r, population[breeder2Index].color.g, population[breeder1Index].color.b);
                tempColor = EvaluateMutation(tempColor);
                childBunny2.SetColor(tempColor);
            }
        }

        // Agrega los nuevos conejos a la poblaci�n existente
        population.AddRange(tempList);
    }

    // Aplica una posible mutaci�n al color del conejo
    public Color EvaluateMutation(Color bunny)
    {
        float rateOfMutation = 0.1f; // Tasa de mutaci�n del 10%
        Vector3 mutatedColor = new Vector3(bunny.r, bunny.g, bunny.b);

        // Recorre los componentes de color y aplica mutaci�n con cierta probabilidad
        for (int i = 0; i < 3; i++)
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) <= rateOfMutation)
            {
                mutatedColor[i] = UnityEngine.Random.Range(0.0f, 1.0f);
            }
        }

        return new Color(mutatedColor.x, mutatedColor.y, mutatedColor.z);
    }

    // Bucle de evaluaci�n peri�dico que llama a EvaluatePopulation cada cierto tiempo
    IEnumerator EvaluationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            EvaluatePopulation();
        }
    }
}
