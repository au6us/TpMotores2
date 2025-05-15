using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class SlicedEntityCounter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI uiText;

    [Header("Time‑Slicing")]
    [SerializeField] private int sliceSize = 20;
    [SerializeField] private float sliceInterval = 0.05f;

    private Coroutine countingRoutine;

    private void OnEnable()
    {
        countingRoutine = StartCoroutine(CountActiveEntities());
    }

    private void OnDisable()
    {
        if (countingRoutine != null)
            StopCoroutine(countingRoutine);
    }

    private IEnumerator CountActiveEntities()
    {
        var waitSlice = new WaitForSeconds(sliceInterval);
        var waitFull = new WaitForSeconds(1f);

        while (true)
        {
            // 1) Recoge **todos** los Entity de la escena
            var allEntities = FindObjectsOfType<Entity>();

            // 2) Filtra sólo los que tengan vida > 0
            var filtered = allEntities
                            .Where(e => e.life > 0)
                            .ToList(); // materializamos la lista

            // 3) Haz time‑slicing para no bloquear el frame
            int total = filtered.Count; // prop Count
            int processed = 0;
            int runningCount = 0;

            while (processed < total)
            {
                // toma el siguiente batch
                var batch = filtered
                                .Skip(processed)
                                .Take(sliceSize);

                runningCount += batch.Count(); // extensión LINQ Count()
                processed += sliceSize;

                yield return waitSlice;        // cede al motor
            }

            // 4) Actualiza UI
            if (uiText != null)
                uiText.text = $"Enemigos vivos: {runningCount}";

            // 5) espera un segundo antes de recuento de nuevo
            yield return waitFull;
        }
    }
}
