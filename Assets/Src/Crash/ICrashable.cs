namespace Crash {
    /// <summary>
    /// 破壊衝突判定に反応するクラスに対して約束されるインターフェース
    /// </summary>
    public interface ICrashable {
        
        /// <summary>
        /// 衝突判定時に呼び出されるメソッド
        /// </summary>
        /// <param name="context"></param>
        void OnCrash (ref CrashCollisionContext context);
        
    }
}