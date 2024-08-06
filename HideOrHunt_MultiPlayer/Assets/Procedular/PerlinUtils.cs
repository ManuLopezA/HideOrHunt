using UnityEngine;

public class PerlinUtils : MonoBehaviour
{
    private PerlinUtils() { }


    public static float CalculatePerlinNoise(float x, float y, float frequency, float offsetX, float offsetY,
        int octaves = 0, float lacunarity = 2, float persistence = 0.5f, bool carveOctaves = true,
        bool verbose = false, bool returnAllValues = false)
    {
        //Per cada casella comprovem soroll perlin donats els par�metres
        // les coordenades x i y que buscarem venen despla�ades per l'offset
        // la freq�encia ens determina com de grans s�n els passos que fem
        float xCoord = offsetX + x / frequency;
        float yCoord = offsetY + y / frequency;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);

        //Valor base
        if (verbose) Debug.Log($"Base: [{x},{y}] = {sample}");

        //Acte seguit calculem les octaves
        for (int octave = 1; octave <= octaves; octave++)
        {
            //La Lacunarity afecta a la freq�encia de cada subseq�ent octava. El limitem a [2,3] de forma
            // que cada nou valor sigui 1/2 o 1/3 de la freq�encia anterior (doble o triple de soroll)
            float newFreq = frequency / (lacunarity * octave);
            float xOctaveCoord = offsetX + x / newFreq;
            float yOctaveCoord = offsetY + y / newFreq;

            //valor base de l'octava
            float octaveSample = Mathf.PerlinNoise(xOctaveCoord, yOctaveCoord);


            //La Persistence afecta a l'amplitud de cada subseq�ent octava. El limitem a [0.1, 0.9] de forma
            // que cada nou valor afecti menys al resultat final.
            //Si Carve Octaves est� actiu ->
            // addicionalment, farem que el soroll en comptes de ser un valor base [0,1] sigui [-0.5f,0.5f]
            // i aix� pugui sumar o restar al valor inicial
            octaveSample = (octaveSample - (carveOctaves ? .5f : 0)) * (persistence / octave);

            //acumulaci� del soroll amb les octaves i base anteriors
            if (verbose) Debug.Log($"Octave {octave}: [{x},{y}] = {octaveSample}");
            sample += octaveSample;
        }

        if (verbose) Debug.Log($"Post octaves: [{x},{y}] = {sample}");

        return sample;
    }
}
