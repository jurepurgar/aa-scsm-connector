using PurgarNET.AAConnector.Shared.AutomationClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PurgarNET.AAConnector.Shared
{
    [DataContract(Name = "ParameterMapping", Namespace = "")]
    public class ParameterMapping : INotifyPropertyChanged
    {
        private string _name = null;
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
        public string PropertyMapping
        {
            get
            {
                return _propertyMapping;
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


    [CollectionDataContract(Name = "ParameterMappings", Namespace = "")]
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
                if (string.IsNullOrEmpty(this[i].Name) || !runbook.Properties.Parameters.ContainsKey(this[i].Name))
                    this.RemoveAt(i);
            }
        }

        public override string ToString()
        {
            var serializer = new DataContractSerializer(this.GetType());
            MemoryStream memoryStream = new MemoryStream();
            serializer.WriteObject(memoryStream, this);
            var str = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            str = str.Replace("\x00", ""); 
            return str;
        }

        public static ParameterMappings CreateFromString(string str)
        {
            var serializer = new DataContractSerializer(typeof(ParameterMappings));
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            try
            {
                var obj = (ParameterMappings)serializer.ReadObject(memoryStream);
                return obj;
            }
            catch {
                return null;
            }

        }

        

    }
}
