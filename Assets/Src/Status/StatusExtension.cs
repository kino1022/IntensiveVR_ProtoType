using R3;

namespace Status {
    public static partial class StatusExtension {
        
        /// <summary>
        /// ステータスが特定の値を下回った場合に流れるストリームを提供する
        /// </summary>
        /// <param name="status"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Observable<Unit> OnLessAnyValue (this IStatus status, float value) {
            return status
                .Current
                .Where(current => current < value)
                .AsUnitObservable();
        }
       
        /// <summary>
        /// ステータスが特定の値を上回った場合に流れるストリームを提供する
        /// </summary>
        /// <param name="status"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Observable<Unit> OnMoreAnyValue (this IStatus status, float value) {
            return status
                .Current
                .Where(current => current > value)
                .AsUnitObservable();
        }
    }
}