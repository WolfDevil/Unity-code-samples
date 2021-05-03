namespace Helpers
{
    public class Singleton<T> where T : class, new()
    {
        #region Private Fields

        private static T _instance;

        #endregion

        #region Properties

        public static T Instance => _instance ?? (_instance = new T());

        #endregion
    }
}