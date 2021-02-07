using System.Collections.Generic;
using System;
using UnityEngine;
using CoreProject.Singleton;

namespace CoreProject.Resource
{
    public class ResourceManager : SingletonClass<ResourceManager>
    {
        #region Fields

        private Dictionary<string, UnityEngine.Object> _resources;

        public ResourceManager()
        {
            if(_resources == null)
            {
                Initialize();
            }
        }

        #endregion

        #region Installation

        /// <summary>
        /// if resources initialze with special folders can use the args, if args is null then manager is loads from root folder.
        /// </summary>
        /// <param name="args">special folders.</param>
        public void Initialize(params string[] args)
        {
            LoadFromResourcesFolder(args);
        }

        private void LoadFromResourcesFolder(params string[] pathArgs)
        {
            _resources = new Dictionary<string, UnityEngine.Object>();
            if (pathArgs != null && pathArgs.Length > 0)
            {
                foreach (var path in pathArgs)
                {
                    LoadResource(path);
                }
            }
            else
            {
                LoadResource("");
            }
        }

        private void LoadResource(string path)
        {
            var resources = Resources.LoadAll("");
            Debug.Log($"Loaded {resources.Length} resource at {(String.IsNullOrEmpty(path) ? "/" : path)}");
            foreach (var resource in resources)
            {
                if (_resources.ContainsKey(resource.name))
                {
                    Debug.LogError($"Name conflict detected! ,{resource.name} was loaded before.");
                }
                else
                {
                    _resources.Add(resource.name, resource);
                }
            }
        }

        #endregion

        #region Interface
        /// <summary>
        /// Get the object at runtime. Object must be in the Resources folder
        /// </summary>
        /// <typeparam name="T">Object Type. UnityEngine.GameObject or those derived from UnityEngine.Object class.</typeparam>
        /// <param name="name">Object name. The Name is must equal to the name in the Resources folder</param>
        /// <returns></returns>
        public T GetResource<T>(string name) where T : UnityEngine.Object
        {
            if (_resources.ContainsKey(name))
            {
                return _resources[name] as T;
            }
            else
            {
                throw new ArgumentNullException(name + ", Resource does not found in resources.");
            }
        }

        #endregion
    }
}