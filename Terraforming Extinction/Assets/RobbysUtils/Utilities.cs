using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace RobbysUtils
{
    public class Utilities : MonoBehaviour
    {
        public void StartTimer(float time, Action endTimerFunction)
        {
            StartCoroutine(TimerCoroutine(time, endTimerFunction));
        }

        IEnumerator TimerCoroutine(float time, Action endTimerFunction)
        {
            bool endTimer = false;
            while (!endTimer)
            {
                if (time <= 0)
                {
                    endTimerFunction.Invoke();
                    endTimer = true;
                }
                else
                    time -= Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }
        }
    }

    public static class BasicUtils
    {
        public static Transform GetClosestTransform(Transform transformToTrack, Transform[] transforms)
        {
            Transform result = null;
            float closestDistance = 0;
            foreach (var transform in transforms)
            {
                if (transform == transformToTrack)
                    continue;

                var distance = TransformUtils.GetDistance(transform.position, transformToTrack.position);
                if (distance < closestDistance || closestDistance == 0)
                {
                    closestDistance = distance;
                    result = transform;
                }
            }

            return result;
        }

        public static Transform GetClosestTransform(Transform transformToTrack, GameObject[] gameObjects)
        {
            Transform result = null;
            float closestDistance = 0;
            foreach (var go in gameObjects)
            {
                if (go.transform == transformToTrack)
                    continue;

                var distance = TransformUtils.GetDistance(go.transform.position, transformToTrack.position);
                if (distance < closestDistance || closestDistance == 0)
                {
                    closestDistance = distance;
                    result = go.transform;
                }
            }

            return result;
        }

        #region CodeMonkey Functions
        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        public static Vector3 GetMouseWorldPosition(Camera camera)
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, camera);
            vec.z = 0f;
            return vec;
        }

        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        // Create Text in the World
        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
        {
            if (color == null) color = Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
        }

        // Create Text in the World
        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }
        #endregion

        public static Vector2 GetPointAlongQuadraticBezierCurve(Vector2 pos0, Vector2 pos1, Vector2 pos2, float t)
        {
            return (Mathf.Pow(1 - t, 2) * pos0) + (2 * (1 - t) * t * pos1) + (Mathf.Pow(t, 2) * pos2);
        }
    }

    public static class ExtensionMethods
    {
        // ConvertTo should do the same thing
        //public static Transform[] ConvertToTransformArray(this Component[] array)
        //{
        //    Transform[] resultArray = new Transform[array.Length];
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        resultArray[i] = array[i].transform;
        //    }

        //    return resultArray;
        //}

        public static int WithRandomSign(this int value, float negativeProbability = 0.5f)
        {
            return UnityEngine.Random.value < negativeProbability ? -value : value;
        }

        public static float WithRandomSign(this float value, float negativeProbability = 0.5f)
        {
            return UnityEngine.Random.value < negativeProbability ? -value : value;
        }

        public static float SnapTo(this float value, float snapToValue)
        {
            return Mathf.Round(value / snapToValue) * snapToValue;
        }
    }

    public static class VectorExtensionMethods
    {
        public static Vector2 xy(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector3 WithX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 WithY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 WithZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector2 WithX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }

        public static Vector2 WithY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }

        public static Vector3 WithZ(this Vector2 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        // axisDirection - unit vector in direction of an axis (eg, defines a line that passes through zero)
        // point - the point to find nearest on line for
        public static Vector3 NearestPointOnAxis(this Vector3 axisDirection, Vector3 point, bool isNormalized = false)
        {
            if (!isNormalized) axisDirection.Normalize();
            var d = Vector3.Dot(point, axisDirection);
            return axisDirection * d;
        }

        // lineDirection - unit vector in direction of line
        // pointOnLine - a point on the line (allowing us to define an actual line in space)
        // point - the point to find nearest on line for
        public static Vector3 NearestPointOnLine(
            this Vector3 lineDirection, Vector3 point, Vector3 pointOnLine, bool isNormalized = false)
        {
            if (!isNormalized) lineDirection.Normalize();
            var d = Vector3.Dot(point - pointOnLine, lineDirection);
            return pointOnLine + (lineDirection * d);
        }

        public static Vector2 SnapTo(this Vector2 value, float snapToValue)
        {
            Vector2 res = new Vector2(value.x.SnapTo(snapToValue), value.y.SnapTo(snapToValue));
            return res;
        }

        public static Vector3 SnapTo(this Vector3 value, float snapToValue)
        {
            Vector3 res = new Vector3(value.x.SnapTo(snapToValue), value.y.SnapTo(snapToValue), value.z.SnapTo(snapToValue));
            return res;
        }
    }

    public static class ArrayExtensions
    {
        public static T[] AddItem<T>(this T[] array, T itemToAppend)
        {
            int newArrayLength = array.Length + 1;
            T[] newArray = new T[newArrayLength];
            for (int i = 0; i < newArrayLength; i++)
            {
                if (i == array.Length)
                    newArray[i] = itemToAppend;
                else
                    newArray[i] = array[i];
            }

            return newArray;
        }

        public static T[] AddItemAtBeginning<T>(this T[] array, T itemToAppend)
        {
            int newArrayLength = array.Length + 1;
            T[] newArray = new T[newArrayLength];
            for (int i = 0; i < newArrayLength; i++)
            {
                if (i == 0)
                    newArray[i] = itemToAppend;
                else
                    newArray[i] = array[i - 1];
            }

            return newArray;
        }

        public static T[] AddMultipleItems<T>(this T[] array, T[] itemsToAppend)
        {
            if (itemsToAppend.Length == 0)
                return array;

            int newArrayLength = array.Length + itemsToAppend.Length;
            T[] newArray = new T[newArrayLength];
            for (int i = 0; i < newArrayLength; i++)
            {
                if (i >= array.Length)
                    newArray[i] = itemsToAppend[i - array.Length];
                else
                    newArray[i] = array[i];
            }

            return newArray;
        }

        public static T[] Clear<T>(this T[] array)
        {
            if (array.Length == 0)
                return array;

            array = new T[0];
            return array;
        }

        public static void Each<T>(this T[] array, Action<T> action)
        {
            foreach (var item in array)
            {
                action?.Invoke(item);
            }
        }

        public static T[] RemoveItem<T>(this T[] array, T itemToRemove)
        {
            if (array.Length == 0)
            {
                return array;
            }

            if (!array.HasItem(itemToRemove))
            {
                Debug.LogError("Item does not exist in array");
                return array;
            }

            int newArrayLength = array.Length - array.NumberOfItem(itemToRemove);
            T[] newArray = new T[newArrayLength];

            int i = 0;
            int j = 0;
            while (i < array.Length)
            {
                if (!array[i].Equals(itemToRemove))
                {
                    newArray[j] = array[i];
                    j++;
                }

                i++;
            }

            return newArray;
        }

        public static T[] RemoveFirstInstanceOfItem<T>(this T[] array, T itemToRemove)
        {
            if (array.Length == 0)
            {
                Debug.LogWarning("Array has no items");
                return array;
            }

            if (!array.HasItem(itemToRemove))
            {
                Debug.LogError("Item does not exist in array");
                return array;
            }

            bool hasItemBeenRemoved = false;

            int newArrayLength = array.Length - 1;
            T[] newArray = new T[newArrayLength];

            int i = 0;
            int j = 0;
            while (i < array.Length)
            {
                if (!array[i].Equals(itemToRemove) || hasItemBeenRemoved)
                {
                    newArray[j] = array[i];
                    j++;
                }
                else
                    hasItemBeenRemoved = true;

                i++;
            }

            return newArray;
        }

        public static T[] RemoveDuplicates<T>(T[] arr)
        {
            List<T> list = new List<T>();
            foreach (T t in arr)
            {
                if (!list.Contains(t))
                {
                    list.Add(t);
                }
            }
            return list.ToArray();
        }

        public static int NumberOfItem<T>(this T[] array, T item)
        {
            if (array.Length == 0)
                return 0;

            int numOfItems = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(item))
                    numOfItems++;
            }

            return numOfItems;
        }

        public static bool HasItem<T>(this T[] array, T item)
        {
            if (array.Length == 0)
                return false;

            foreach (T element in array)
            {
                if (element.Equals(item))
                    return true;
            }

            return false;
        }

        public static int IndexOf<T>(this T[] array, T item)
        {
            if (array.Length == 0)
                return -1;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(item))
                    return i;
            }

            return -1;
        }

        public static int[] IndicesOf<T>(this T[] array, T item)
        {
            if (array.Length == 0)
                return new int[] { };

            int[] indices = new int[0];
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(item))
                    indices = indices.AddItem(i);
            }

            return indices;
        }

        public static T RandomItem<T>(this T[] array)
        {
            if (array.Length == 0)
                throw new IndexOutOfRangeException();

            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        public static T[] ConvertTo<T>(this Component[] array) where T : Component
        {
            if (array.Length == 0)
                return new T[] { };

            T[] resultArray = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                resultArray[i] = array[i].GetComponent<T>();
            }

            return resultArray;
        }

        public static T LastItem<T>(this T[] array)
        {
            if (array.Length == 0)
                return default(T);

            return array[array.Length - 1];
        }

        public static T[] RemoveAllButFirstItem<T>(this T[] array)
        {
            if (array.Length == 0)
                return new T[] { };

            T[] resultArray = new T[1];
            resultArray[0] = array[0];

            return resultArray;
        }

        public static T[] GetFirstNItems<T>(this T[] array, int n)
        {
            if (array.Length == 0)
                return new T[] { };
            
            int valueToRunTo = array.Length < n ? array.Length : n;

            T[] resultArray = new T[0];
            for (int i = 0; i < valueToRunTo; i++)
            {
                resultArray.AddItem(array[i]);
            }

            return resultArray;
        }
    }

    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    public static class TransformUtils
    {
        public static float GetDistance(Vector3 startPos, Vector3 endPos)
        {
            return (startPos - endPos).magnitude;
        }

        public static Vector2 GetMidPoint(Vector2 startPos, Vector2 endPos)
        {
            return (startPos + endPos) / 2f;
        }

        public static Vector3 GetMidPoint(Vector3 startPos, Vector3 endPos)
        {
            return (startPos + endPos) / 2f;
        }

        public static Vector3 GetAveragePoint(this Vector3[] points)
        {
            float x = 0, y = 0, z = 0;
            foreach (var point in points)
            {
                x += point.x;
                y += point.y;
                z += point.z;
            }

            return new Vector3(x, y, z) / points.Length;
        }



        public static bool LerpTo(this Transform transformToMove, Vector2 point, bool useLocalSpace, float animationSpeed = 1f, float minimumDistance = 0.01f)
        {
            if (useLocalSpace)
            {
                transformToMove.localPosition = Vector2.Lerp(transformToMove.localPosition, point, Time.deltaTime * animationSpeed);
                return GetDistance((Vector2)transformToMove.localPosition, point) < minimumDistance;
            }
            else
            {
                transformToMove.position = Vector2.Lerp(transformToMove.position, point, Time.deltaTime * animationSpeed);
                return GetDistance((Vector2)transformToMove.position, point) < minimumDistance;
            }
        }

        public static bool LerpToPoint(Transform transformToMove, Vector2 point, bool useLocalSpace, float animationSpeed = 1f, float minimumDistance = 0.01f)
        {
            if (useLocalSpace)
            {
                transformToMove.localPosition = Vector2.Lerp(transformToMove.localPosition, point, Time.deltaTime * animationSpeed);
                return GetDistance((Vector2)transformToMove.localPosition, point) < minimumDistance;
            }
            else
            {
                transformToMove.position = Vector2.Lerp(transformToMove.position, point, Time.deltaTime * animationSpeed);
                return GetDistance((Vector2)transformToMove.position, point) < minimumDistance;
            }
        }

        public static bool LerpLocalPostionToPoint(Transform transformToMove, Vector2 point, float animationSpeed = 1f, float minimumDistance = 0.01f)
        {
            transformToMove.localPosition = Vector2.Lerp(transformToMove.localPosition, point, Time.deltaTime * animationSpeed);
            return GetDistance((Vector2)transformToMove.localPosition, point) < minimumDistance;
        }

        public static bool LerpPostionToPoint(Transform transformToMove, Vector2 point, float animationSpeed = 1f, float minimumDistance = 0.01f)
        {
            transformToMove.position = Vector2.Lerp(transformToMove.position, point, Time.deltaTime * animationSpeed);
            return GetDistance((Vector2)transformToMove.position, point) < minimumDistance;
        }

        public static bool LerpLocalRotation(Transform transformToRotate, Quaternion rotation, float animationSpeed = 1f, float minimumDistance = 0.001f)
        {
            transformToRotate.localRotation = Quaternion.Lerp(transformToRotate.localRotation, rotation, Time.deltaTime * animationSpeed);
            return GetDistance(transformToRotate.localRotation.eulerAngles, rotation.eulerAngles) < minimumDistance;
        }

        public static bool LerpLocalScaleTo(Transform transformToLerp, float scale, float animationSpeed = 1f, float minimumDistance = 0.001f)
        {
            Vector3 endScale = Vector3.one * scale;
            transformToLerp.localScale = Vector3.Lerp(transformToLerp.localScale, endScale, Time.deltaTime * animationSpeed);
            return GetDistance(transformToLerp.localScale, endScale) < minimumDistance;
        }
    }

    public static class CameraUtils
    {
        /// <summary>
        /// Manually change this to show debug messages
        /// </summary>
        public static bool ShowDebugInfo = false;
        public static CameraBounds cachedCameraBounds;

        /// <summary>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Ordering</term>
        ///     </listheader>
        ///     <item>
        ///         <description>Left=[0]</description>
        ///     </item>
        ///     <item>
        ///         <description>Right=[1]</description>
        ///     </item>
        ///     <item>
        ///         <description>Down=[2]</description>
        ///     </item>
        ///     <item>
        ///         <description>Up=[3]</description>
        ///     </item>
        ///     <item>
        ///         <description>Near=[4]</description>
        ///     </item>
        ///     <item>
        ///         <description>Far=[5]</description>
        ///     </item>
        /// </list>
        /// </summary>
        public class CameraBounds
        {
            public Vector3 LeftBound { get { return bounds[0]; } }
            public Vector3 RightBound { get { return bounds[1]; } }
            public Vector3 DownBound { get { return bounds[2]; } }
            public Vector3 UpBound { get { return bounds[3]; } }
            public Vector3 NearBound { get { return bounds[4]; } }
            public Vector3 FarBound { get { return bounds[5]; } }

            public Camera associatedCamera;

            public Vector3[] bounds = new Vector3[6];
        }

        public static CameraBounds GetCameraViewBounds(Camera camera)
        {
            if (cachedCameraBounds != null && cachedCameraBounds.associatedCamera == camera)
            {
#if UNITY_EDITOR
                if (ShowDebugInfo)
                    Debug.Log("Using Cached");
#endif
                return cachedCameraBounds;
            }
            else
            {
#if UNITY_EDITOR
                if (ShowDebugInfo)
                    Debug.Log("Not Using Cached");
#endif
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

                Vector3[] results = new Vector3[6];
                for (int i = 0; i < 6; ++i)
                {
                    results[i] = -planes[i].normal * planes[i].distance;
                }

                CameraBounds cameraBounds = new CameraBounds() { bounds = results, associatedCamera = camera };
                cachedCameraBounds = cameraBounds;
                return cameraBounds;
            }
        }

        public static CameraBounds GetCameraViewBounds()
        {
            return GetCameraViewBounds(Camera.main);
        }

        public static bool IsWithinCameraView2D(Transform transform, Camera camera)
        {
            CameraBounds cameraBounds = GetCameraViewBounds(camera);

            bool result = true;
            if (transform.position.x > cameraBounds.RightBound.x)
                result = false;
            else if (transform.position.x < cameraBounds.LeftBound.x)
                result = false;

            if (transform.position.y > cameraBounds.UpBound.y)
                result = false;
            else if (transform.position.y < cameraBounds.DownBound.y)
                result = false;

            return result;
        }

        public static bool IsWithinCameraView2D(Transform transform)
        {
            return IsWithinCameraView2D(transform, Camera.main);
        }

        public static void PreventLeavingBounds(Transform transform)
        {
            CameraUtils.CameraBounds cameraBounds = CameraUtils.GetCameraViewBounds();

            if (transform.position.x > cameraBounds.RightBound.x)
                transform.position = new Vector2(cameraBounds.RightBound.x, transform.position.y);
            else if (transform.position.x < cameraBounds.LeftBound.x)
                transform.position = new Vector2(cameraBounds.LeftBound.x, transform.position.y);

            if (transform.position.y > cameraBounds.UpBound.y)
                transform.position = new Vector2(transform.position.x, cameraBounds.UpBound.y);
            else if (transform.position.y < cameraBounds.DownBound.y)
                transform.position = new Vector2(transform.position.x, cameraBounds.DownBound.y);
        }
    }

    public static class StringUtils
    {
        public static string Format(this string baseString, params string[] args)
        {
            StringBuilder sb = new StringBuilder(baseString);
            for (int x = 0; x < args.Length; x++)
            {
                sb.Append(args[x]);
            }
            return sb.ToString();
        }
    }

    public static class MathUtils
    {
        public static float RoundToNearestTenth(float num)
        {
            num *= 10;
            num = Mathf.RoundToInt(num);
            num /= 10;

            return num;
        }

        public static float RoundToNearestHundredth(float num)
        {
            num *= 50;
            num = Mathf.RoundToInt(num);
            num /= 50;

            return num;
        }
    }

    public static class GUIUtils
    {
        public static void DrawButton(string labelText, Action buttonFunc)
        {
            GUILayout.BeginHorizontal();
            if (UnityEngine.GUILayout.Button(labelText))
                buttonFunc?.Invoke();
            GUILayout.EndHorizontal();
        }

        public static void DrawLabel(string labelText)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(labelText);
            GUILayout.EndHorizontal();
        }
    }
}