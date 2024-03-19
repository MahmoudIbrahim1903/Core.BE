using System;

namespace Emeint.Core.BE.Domain.Managers
{
    public class BaseManager<T> where T : new()
    {
        public T Instance => Activator.CreateInstance<T>();

        // protected EntityManager entityManager;

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion
    }
}