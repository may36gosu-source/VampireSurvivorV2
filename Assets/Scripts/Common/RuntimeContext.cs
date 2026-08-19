using UnityEngine;
namespace VampireSurvivors.Common
{

    public class RuntimeContext {

        public T Find<T>() where T: Object
        {
            
            T result = Object.FindFirstObjectByType<T>();

            if(result == null)
            {
                Debug.LogError($"[RuntimeContext] " + $"Không tìm thấy dependency: {typeof(T).Name}");
            }
            return result;
        }
    }
}