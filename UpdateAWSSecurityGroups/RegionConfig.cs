using Amazon;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SetMySecurityGroup {
	public class RegionConfig : ConfigurationElement {
		public RegionConfig() { }

		[ConfigurationProperty("endPoint", IsRequired = true, IsKey = true)]
		public string EndPoint {
			get { return (string)this["endPoint"]; }
			set { this["endPoint"] = value; }
		}
		public RegionEndpoint RegionEndpoint {
			get {
				if (string.IsNullOrWhiteSpace(EndPoint)) { return null; }
				if (RegionEndpoint.GetBySystemName(EndPoint).DisplayName == "Unknown") {
					throw new ArgumentException("Unknown region: " + EndPoint);
				}
				return RegionEndpoint.GetBySystemName(EndPoint);
			}
		}
		[ConfigurationProperty("accessKey", IsRequired = true)]
		public string AccessKey {
			get { return (string)this["accessKey"]; }
			set { this["accessKey"] = value; }
		}

		[ConfigurationProperty("accessKeySecret", IsRequired = true)]
		public string AccessKeySecret {
			get { return (string)this["accessKeySecret"]; }
			set { this["accessKeySecret"] = value; }
		}

		[ConfigurationProperty("securityGroupIds", IsRequired = true)]
		public string _SecurityGroupIds {
			get { return (string)this["securityGroupIds"]; }
			set { this["securityGroupIds"] = value; }
		}
		public IEnumerable<string> SecurityGroupIds {
			get { return _SecurityGroupIds.Split(','); }
			set { _SecurityGroupIds = string.Join(",", value); }
		}

	}




	public class AWSRegionsConfigurationSection : ConfigurationSection {
		public static AWSRegionsConfigurationSection Get {
			get {
				return ConfigurationManager.GetSection("AWS") as AWSRegionsConfigurationSection; ;
			}
		}

		[ConfigurationProperty("Regions", IsDefaultCollection = false)]
		[ConfigurationCollection(typeof(RegionCollection),
			AddItemName = "add",
			ClearItemsName = "clear",
			RemoveItemName = "remove")]
		public RegionCollection Regions {
			get {
				return (RegionCollection)base["Regions"];
			}
		}
	}

	public class RegionCollection : ConfigurationElementCollection {
		public RegionCollection() { }

		public RegionConfig this[int index] {
			get { return (RegionConfig)BaseGet(index); }
			set {
				if (BaseGet(index) != null) {
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

		public void Add(RegionConfig depConfig) {
			BaseAdd(depConfig);
		}

		public void Clear() {
			BaseClear();
		}

		protected override ConfigurationElement CreateNewElement() {
			return new RegionConfig();
		}

		protected override object GetElementKey(ConfigurationElement element) {
			return ((RegionConfig)element).EndPoint;
		}

		public void Remove(RegionConfig depConfig) {
			BaseRemove(depConfig.EndPoint);
		}

		public void RemoveAt(int index) {
			BaseRemoveAt(index);
		}

		public void Remove(string name) {
			BaseRemove(name);
		}
	}

}
