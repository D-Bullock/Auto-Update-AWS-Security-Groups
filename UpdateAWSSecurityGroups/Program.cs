using Amazon.EC2;
using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SetMySecurityGroup {
	class Program {
		static readonly string myIpRange = new WebClient().DownloadString("http://checkip.amazonaws.com/").Replace("\n", "") + "/32";

		static void Main(string[] args) {
			try {
				foreach (RegionConfig r in AWSRegionsConfigurationSection.Get.Regions) {
					using (var client = new AmazonEC2Client(r.AccessKey, r.AccessKeySecret, r.RegionEndpoint)) {
						Console.WriteLine("Region " + r.RegionEndpoint.DisplayName);
						var securityGroups = GetSecurityGroups(client, r.SecurityGroupIds);
						foreach (var securityGroup in securityGroups) {
							Console.WriteLine("\tUpdating  {0} ({1})", securityGroup.GroupId, securityGroup.GroupName);
							RevokeExistingPermissions(client, securityGroup);
							AuthorizeNewPermissions(client, securityGroup);
						}
					}
				}

			} catch (Exception x) {
				Console.WriteLine(x.ToString());
				Console.ReadKey();
			}

			foreach (RegionConfig d in AWSRegionsConfigurationSection.Get.Regions) {

			}
		}

		static IEnumerable<SecurityGroup> GetSecurityGroups(AmazonEC2Client client, IEnumerable<string> groupIds) {
			return client.DescribeSecurityGroups(new DescribeSecurityGroupsRequest {
				GroupIds = groupIds.ToList(),
			}).SecurityGroups;
		}

		static void RevokeExistingPermissions(AmazonEC2Client client, SecurityGroup securityGroup) {
			var permissionsToRevoke = securityGroup.IpPermissions.Where(p => p.FromPort == 0 && p.ToPort == 65535).ToList();
			if (permissionsToRevoke.Count > 0) {
				var requestRevoke = new RevokeSecurityGroupIngressRequest {
					GroupId = securityGroup.GroupId,
					IpPermissions = permissionsToRevoke
				};
				client.RevokeSecurityGroupIngress(requestRevoke);
			}
		}

		static void AuthorizeNewPermissions(AmazonEC2Client client, SecurityGroup securityGroup) {
			var requestAuthorize = new AuthorizeSecurityGroupIngressRequest {
				GroupId = securityGroup.GroupId,
				IpPermissions = new List<IpPermission> {
							new IpPermission {
								FromPort = 0,
								ToPort = 65535,
								IpProtocol = "TCP",
								IpRanges = new List<string> { myIpRange },
							},
						},
			};
			client.AuthorizeSecurityGroupIngress(requestAuthorize);
		}
	}
}
