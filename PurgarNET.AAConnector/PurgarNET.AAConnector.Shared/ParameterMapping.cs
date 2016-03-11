using PurgarNET.AAConnector.Shared.AutomationClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared
{
    public class ParameterMapping : INotifyPropertyChanged
    {
        private string _name = null;
        public string Name {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }

        private bool _isMandatory = false;
        public bool IsMandatory
        {
            get
            {
                return _isMandatory;
            }
            set
            {
                if (_isMandatory != value)
                {
                    _isMandatory = value;
                    NotifyPropertyChanged(nameof(IsMandatory));
                }
            }
        }

        private string _type = null;
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    NotifyPropertyChanged(nameof(Type));
                }
            }

        }

        /*private object _defaultValue = null;
        public object DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                if (_defaultValue != value)
                {
                    _defaultValue = value;
                    NotifyPropertyChanged(nameof(DefaultValue));
                }
            }
        }*/

        private string _propertyMapping = null;
        public string PropertyMapping
        {
            get
            {
                return _propertyMapping = null;
            }
            set
            {
                if (_propertyMapping != value)
                {
                    _propertyMapping = value;
                    NotifyPropertyChanged(nameof(PropertyMapping));
                }
            }
        }

        private void NotifyPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }



    public class ParameterMappings : System.Collections.ObjectModel.ObservableCollection<ParameterMapping> 
    {
        public void UpdateRunbookParameters(Runbook runbook)
        {
            foreach (var param in runbook.Properties.Parameters)
            {
                var item = this.FirstOrDefault(x => x.Name == param.Key);
                if (item == null)
                {
                    item = new ParameterMapping();
                    Add(item);
                }

                item.Name = param.Key;
                item.IsMandatory = param.Value.IsMandatory;
                if (item.Type != param.Value.Type)
                {
                    item.Type = param.Value.Type;
                    //TODO: reset mapping if type does not match
                }
            }

            for (var i = Count - 1; i >= 0; i--)
            {
                if (!runbook.Properties.Parameters.ContainsKey(this[i].Name))
                    this.RemoveAt(i);
            }
        }

        public override string ToString()
        {
            return null; //TODO: serialize to string
        }

        public static ParameterMappings CreateFromString(string str)
        {
            //TODO: deserialize from string
            return null;
        }

        

    }
}
