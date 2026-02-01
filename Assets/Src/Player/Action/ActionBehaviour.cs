using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Player.Action {
    
    public interface IAction<T> where T : struct {

        /// <summary>
        /// 入力が開始された際に呼ばれるメソッド
        /// </summary>
        /// <param name="context"></param>
        void StartAction(ref ActionContext<T> context);
        
        /// <summary>
        /// 入力が行われている間に呼ばれるメソッド
        /// </summary>
        /// <param name="context"></param>
        void PerformAction(ref ActionContext<T> context);
        
        /// <summary>
        /// 入力が終了した際に呼ばれるメソッド
        /// </summary>
        /// <param name="context"></param>
        void CancelAction(ref ActionContext<T> context);

    }
    
    public readonly struct ActionContext<T> where T : struct {
        public readonly T Value;
        public ActionContext(T value) {
            Value = value;
        }
    }

    
    public abstract class ActionBehaviour<T> : SerializedMonoBehaviour, IAction<T> where T : struct {

        public abstract void StartAction(ref ActionContext<T> context);

        public abstract void PerformAction(ref ActionContext<T> context);
        
        public abstract void CancelAction(ref ActionContext<T> context);
        
    }
}