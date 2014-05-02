#if !NETFX_CORE
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NServiceKit.Logging;

namespace NServiceKit.Common.Extensions
{
    /// <summary>
    /// Useful IPAddressExtensions from: 
    /// http://blogs.msdn.com/knom/archive/2008/12/31/ip-address-calculations-with-c-subnetmasks-networks.aspx
    /// 
    /// </summary>
    public static class IPAddressExtensions
    {
        /// <summary>The IPAddress extension method that gets broadcast address.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="address">   The address.</param>
        /// <param name="subnetMask">The subnet mask.</param>
        ///
        /// <returns>The broadcast address.</returns>
        public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
        {
            var ipAdressBytes = address.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            var broadcastAddress = new byte[ipAdressBytes.Length];
            for (var i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        /// <summary>The IPAddress extension method that gets network address.</summary>
        ///
        /// <param name="address">   The address.</param>
        /// <param name="subnetMask">The subnet mask.</param>
        ///
        /// <returns>The network address.</returns>
        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            var ipAdressBytes = address.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();

            return new IPAddress(GetNetworkAddressBytes(ipAdressBytes, subnetMaskBytes));
        }

        /// <summary>Gets network address bytes.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="ipAdressBytes">  The IP adress in bytes.</param>
        /// <param name="subnetMaskBytes">The subnet mask in bytes.</param>
        ///
        /// <returns>An array of byte.</returns>
        public static byte[] GetNetworkAddressBytes(byte[] ipAdressBytes, byte[] subnetMaskBytes) 
        {
            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            var broadcastAddress = new byte[ipAdressBytes.Length];
            for (var i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return broadcastAddress;
        }

        /// <summary>A byte[] extension method that query if 'address1Bytes' is in same IPv 6 subnet.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="address2">The address2 to act on.</param>
        /// <param name="address"> The address.</param>
        ///
        /// <returns>true if in same IPv 6 subnet, false if not.</returns>
        public static bool IsInSameIpv6Subnet(this IPAddress address2, IPAddress address)
        {
            if (address2.AddressFamily != AddressFamily.InterNetworkV6 || address.AddressFamily != AddressFamily.InterNetworkV6)
            {
                throw new ArgumentException("Both IPAddress must be IPV6 addresses");
            }
            var address1Bytes = address.GetAddressBytes();
            var address2Bytes = address2.GetAddressBytes();

            return IsInSameIpv6Subnet(address1Bytes, address2Bytes);
        }

        /// <summary>A byte[] extension method that query if 'address1Bytes' is in same IPv 6 subnet.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="address1Bytes">The address1Bytes to act on.</param>
        /// <param name="address2Bytes">The address 2 in bytes.</param>
        ///
        /// <returns>true if in same IPv 6 subnet, false if not.</returns>
        public static bool IsInSameIpv6Subnet(this byte[] address1Bytes, byte[] address2Bytes) 
        {
            if (address1Bytes.Length != address2Bytes.Length)
                throw new ArgumentException("Lengths of IP addresses do not match.");

            for (var i = 0; i < 8; i++)
            {
                if (address1Bytes[i] != address2Bytes[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>A byte[] extension method that query if 'address1Bytes' is in same IPv 4 subnet.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="address2">  The address2 to act on.</param>
        /// <param name="address">   The address.</param>
        /// <param name="subnetMask">The subnet mask.</param>
        ///
        /// <returns>true if in same IPv 4 subnet, false if not.</returns>
        public static bool IsInSameIpv4Subnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
        {
            if (address2.AddressFamily != AddressFamily.InterNetwork || address.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new ArgumentException("Both IPAddress must be IPV4 addresses");
            }
            var network1 = address.GetNetworkAddress(subnetMask);
            var network2 = address2.GetNetworkAddress(subnetMask);

            return network1.Equals(network2);
        }

        /// <summary>A byte[] extension method that query if 'address1Bytes' is in same IPv 4 subnet.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="address1Bytes">  The address1Bytes to act on.</param>
        /// <param name="address2Bytes">  The address 2 in bytes.</param>
        /// <param name="subnetMaskBytes">The subnet mask in bytes.</param>
        ///
        /// <returns>true if in same IPv 4 subnet, false if not.</returns>
        public static bool IsInSameIpv4Subnet(this byte[] address1Bytes, byte[] address2Bytes, byte[] subnetMaskBytes)
        {
            if (address1Bytes.Length != address2Bytes.Length)
                throw new ArgumentException("Lengths of IP addresses do not match.");

            var network1Bytes = GetNetworkAddressBytes(address1Bytes, subnetMaskBytes);
            var network2Bytes = GetNetworkAddressBytes(address2Bytes, subnetMaskBytes);

            return network1Bytes.AreEqual(network2Bytes);
        }


        /// <summary>
        /// Gets the ipv4 addresses from all Network Interfaces that have Subnet masks.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<IPAddress, IPAddress> GetAllNetworkInterfaceIpv4Addresses()
        {
            var map = new Dictionary<IPAddress, IPAddress>();

            try
            {
#if !SILVERLIGHT 
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (var uipi in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (uipi.Address.AddressFamily != AddressFamily.InterNetwork) continue;

                        if (uipi.IPv4Mask == null) continue; //ignore 127.0.0.1
                        map[uipi.Address] = uipi.IPv4Mask;
                    }
                }
#endif
            }
            catch /*(NotImplementedException ex)*/
            {
                //log.Warn("MONO does not support NetworkInterface.GetAllNetworkInterfaces(). Could not detect local ip subnets.", ex);
            } 
            return map;
        }

        /// <summary>
        /// Gets the ipv6 addresses from all Network Interfaces.
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetAllNetworkInterfaceIpv6Addresses()
        {
            var list = new List<IPAddress>();

            try
            {
#if !SILVERLIGHT 
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (var uipi in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (uipi.Address.AddressFamily != AddressFamily.InterNetworkV6) continue;
                        list.Add(uipi.Address);
                    }
                }
#endif
            }
            catch /*(NotImplementedException ex)*/
            {
                //log.Warn("MONO does not support NetworkInterface.GetAllNetworkInterfaces(). Could not detect local ip subnets.", ex);
            }
            
            return list;
        }

    }
}
#endif