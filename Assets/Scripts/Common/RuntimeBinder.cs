using UnityEngine;

namespace VampireSurvivors.Common
{
  
    public abstract class RuntimeBinder : MonoBehaviour {
        #region Initialized
            // các class con kế thừa bắt buộc phải có hàm Initialized
        #endregion
        public abstract void Initialize(RuntimeContext context);
    }
    
}