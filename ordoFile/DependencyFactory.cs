using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ordoFile.GUITools;
using Microsoft.Practices.Unity;
using ordoFile.DataAccess;

namespace ordoFile
{
    class DependencyFactory
    {
        // Static field to hold UnityContainer instance
        static IUnityContainer myContainer;

        /// <summary>
        /// Static constructor for instantiating UnityContainer field
        /// </summary>
        static DependencyFactory()
        {
            myContainer = new UnityContainer();
        }

        /// <summary>
        /// Static getter for retrieving UnityContainer
        /// </summary>
        public static IUnityContainer Container
        {
            get { return myContainer; }
        }
    }
}
