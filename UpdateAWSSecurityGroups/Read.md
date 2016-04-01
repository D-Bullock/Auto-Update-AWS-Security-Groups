#Set My Security Group
This program is designed to easily set your IP across a range of AWS regions so you can both maintain a secure firewall and change networks/IPs.

##AWS Setup
The key to this setup is having a security group dedicated to just storing your IP, and grants access to all traffic. This security group is then referenced by the actually used security groups, with only the required ports being open. (I'll upload screenshots one day... Post an issue if you don't get it.)

## Application Setup
Once you've set up AWS all you need to do is add lines to the app config. There are already example lines there.


###RoadMap
Change the structure of the app.config to allow the same region with differing access codes. A flaw in the current implementation is the row key is the region code.