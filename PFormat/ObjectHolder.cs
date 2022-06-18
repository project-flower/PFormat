namespace PFormat
{
    /// <summary>
    /// UserControl 等を ComboBox.Items に設定すると
    /// ToString() メソッドをオーバーライドした戻り値がドロップダウンに表示されないため、
    /// このクラスで代用する。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectHolder<T>
    {
        #region Public Properties

        public T Instance { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return Instance.ToString();
        }

        #endregion
    }
}
