using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbysUtils
{
    public class Slider : MonoBehaviour
    {
        [Header("Slider Objects")]
        [SerializeField] Transform sliderBase;
        [SerializeField] Transform sliderPoint;
        [SerializeField] TextMesh sliderText;

        [Header("Settings")]
        [SerializeField] bool useWholeNumbers;
        [SerializeField] bool showNumberText;

        [Header("Input Settings")]
        [SerializeField] float acceptedInputHeight = 60;
        [SerializeField] float acceptedInputWidth = 9;

        [Header("Debugging")]
        [SerializeField] float totalNumOfPoints;
        [SerializeField] float currentPoint;
        [SerializeField] float currentPointAsDecimal;

        public float CurrentValue
        {
            get => currentPoint;
            set
            {
                currentPoint = value;
                SetPoint(currentPoint);
            }
        }

        public float TotalNumOfPoints
        {
            get => totalNumOfPoints;
            set
            {
                totalNumOfPoints = value;
                SetPoint(currentPoint);
            }
        }

        public float CurrentValueAsDecimal 
        { 
            get => currentPointAsDecimal; 
            set => currentPointAsDecimal = value; 
        }

        void Start()
        {
            SetPoint(sliderPoint.position.x);

            sliderText.gameObject.SetActive(showNumberText);
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                var direction = transform.position - worldPoint;
                if ((direction.x > -acceptedInputWidth && direction.x < acceptedInputWidth) && (direction.y > -acceptedInputHeight && direction.y < acceptedInputHeight))
                {
                    SetPoint(worldPoint.x);
                }
            }
        }

        public void SetPoint(float point)
        {
            var temp = point - transform.position.x;
            temp *= totalNumOfPoints / 100f;

            var newPoint = temp / totalNumOfPoints;
            var x = newPoint * 2;

            if (useWholeNumbers)
            {
                x = MathUtils.RoundToNearestHundredth(x);
            }

            if (x > 1)
                x = 1;
            else if (x < -1)
                x = -1;

            currentPoint = (x) * (totalNumOfPoints / 2) + (totalNumOfPoints / 2);
            currentPointAsDecimal = currentPoint / totalNumOfPoints;

            sliderPoint.localPosition = new Vector2(x, 0);
            sliderText.text = currentPoint.ToString();
        }
    }
}