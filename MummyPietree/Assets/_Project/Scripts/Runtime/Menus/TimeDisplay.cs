using Etienne;
using System;
using System.Collections;
using UnityEngine;

namespace MummyPietree
{
    public class TimeDisplay : Singleton<TimeDisplay>
    {
        public event Action OnEndOfDay;

        [SerializeField] private TMPro.TextMeshProUGUI hourTMP, dayTMP;
        [SerializeField] private float durationOfADay = 1f;
        [SerializeField, ReadOnly] private string formattedTime;
        [SerializeField, ReadOnly] private float time;

        float additionnalScale =1;
        TimeSpan timeSpan;

        private void Update()
        {
            var oldTimeSpan = timeSpan;
            time += Time.deltaTime * durationOfADay * additionnalScale;
            timeSpan = TimeSpan.FromSeconds(time);
            formattedTime = $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}";
            hourTMP.text = formattedTime;
            dayTMP.text = $"Day {timeSpan.Days + 1}";
            if (timeSpan.Days > oldTimeSpan.Days)
            {
                OnEndOfDay?.Invoke();
            }
        }

        public void ScaleTime(float scale)
        {
            additionnalScale = scale;
        }

        public void ResetScaledTime()
        {
            additionnalScale = 1f;   
        }


    }
}
