// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework
{
    public abstract class InstanceRepository : INotifyInstancesSstateChanged
    {
        protected Dictionary<string, List<DomainClassDef>> domainInstances = new Dictionary<string, List<DomainClassDef>>();
        protected Dictionary<string, ExternalEntityDef> externalEntities = new Dictionary<string, ExternalEntityDef>();

        public abstract event ClassPropertiesUpdateHandler ClassPropertiesUpdated;
        public abstract event RelationshipUpdateHandler RelationshipUpdated;

        public IEnumerable<string> GetDomainNames()
        {
            return domainInstances.Keys;
        }

        public void Add(DomainClassDef instance)
        {
            if (!domainInstances.ContainsKey(instance.ClassName))
            {
                domainInstances.Add(instance.ClassName, new List<DomainClassDef>());
            }
            domainInstances[instance.ClassName].Add(instance);
        }

        public bool Delete(DomainClassDef instance)
        {
            bool result = false;

            if (domainInstances.ContainsKey(instance.ClassName))
            {
                if (domainInstances[instance.ClassName].Contains(instance))
                {
                    domainInstances[instance.ClassName].Remove(instance);
                    if (domainInstances[instance.ClassName].Count == 0)
                    {
                        domainInstances.Remove(instance.ClassName);
                    }
                    result = true;
                }
            }

            return result;
        }

        public IEnumerable<DomainClassDef> GetDomainInstances(string domainName)
        {
            List<DomainClassDef> result = new List<DomainClassDef>();

            if (domainInstances.ContainsKey(domainName))
            {
                var instances = domainInstances[domainName];
                foreach (var instance in instances)
                {
                    result.Add(instance);
                }
            }

            return result;
        }

        public IList<ChangedState> CreateChangedStates()
        {
            return new List<ChangedState>();
        }

        public void SyncChangedStates(IList<ChangedState> changedStates)
        {
            lock (domainInstances)
            {
                foreach (var changedState in changedStates)
                {
                    if (changedState is CInstanceChagedState)
                    {
                        UpdateCInstance((CInstanceChagedState)changedState);
                    }
                    else if (changedState is CLinkChangedState)
                    {
                        UpdateCLink((CLinkChangedState)changedState);
                    }
                }
                foreach (var className in domainInstances.Keys)
                {
                    foreach (var instance in domainInstances[className])
                    {
                        var updatedState = instance.ChangedProperties();
                        UpdateState(instance, updatedState);
                    }
                }
            }
        }

        public void UpdateState()
        {
            foreach (var className in domainInstances.Keys)
            {
                foreach (var instance in domainInstances[className])
                {
                    var changedStates = instance.ChangedProperties();
                    UpdateState(instance, changedStates);
                }
            }
        }

        ///
        /// Update stored state of the instance by changed argument.
        /// changed.key is name of property of the instance.
        /// changed.value is value of the property that the name of it  is changed.key
        ///
        public abstract void UpdateState(DomainClassDef instance, IDictionary<string, object> chnaged);

        ///
        /// Construct state of the instances by instances argument.
        /// instances.key is domain class name.
        /// instances.value is instances states of the domain class.
        /// each item of the instances.value is property name and value pairs.
        ///
        public abstract void LoadState(string domainName, IDictionary<string, IList<IDictionary<string, object>>> instances);

        public abstract void UpdateCInstance(CInstanceChagedState instanceState);
        public abstract void UpdateCLink(CLinkChangedState linkState);

        public abstract IEnumerable<T> SelectInstances<T>(string className, IDictionary<string, object> conditionPropertyValues, Func<T, IDictionary<string, object>, bool> compare) where T : DomainClassDef;

        public void Add(ExternalEntityDef externalEntity)
        {
            externalEntities.Add(externalEntity.EEKey, externalEntity);
        }

        public ExternalEntityDef GetExternalEntity(string eeKey)
        {
            ExternalEntityDef eeDef = null;
            if (externalEntities.ContainsKey(eeKey))
            {
                eeDef = externalEntities[eeKey];
            }
            return eeDef;
        }
    }

}
