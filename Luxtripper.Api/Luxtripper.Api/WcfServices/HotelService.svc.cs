using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Luxtripper.Api.WcfServices
{
    /// <summary>
    /// This is the Service. If you want to use it your credentials are = UserName: "DiscoCat", Password: "NinjaMoves"...
    /// </summary>
    public class HotelService : IHotelService
    {
        /// <summary>
        /// You first have to log in to get the token.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            if (request.UserName != "DiscoCat" || request.Password != "NinjaMoves")
            {
                return new LoginResponse
                {
                    Status = new Status
                    {
                        Error =
                            "Not Authorized: Your credentials are = UserName: DiscoCat, Password: NinjaMoves"
                    }
                };
            }
            return new LoginResponse
            {
                Status= new Status {Success = true},
                UserName = request.UserName,
                Token = "A9-SDTES-NIC0-WR132-412422"
            };
        }

        /// <summary>
        /// When you've got the token from the Login Call then you are free to use the API.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<HotelResponse> GetHotels(HotelRequest request)
        {
            if (request.Security.Token == null)
            {
                return new HotelResponse
                {
                    Status = new Status
                    {
                        Error =
                            "Missing Token: You first have to login with your credentials (UserName: DiscoCat, Password: NinjaMoves)"
                    }
                };
            }
            if (request.Security.UserName != "DiscoCat" || request.Security.Token != "A9-SDTES-NIC0-WR132-412422")
            {
                return new HotelResponse
                {
                    Status = new Status
                    {
                        Error =
                            "Illegal Token: You first have to login with your credentials (UserName: DiscoCat, Password: NinjaMoves)"
                    }
                };
            }
            try
            {
                return new HotelResponse
                {
                    Status = new Status {Success = true},
                    Hotels = new[]
                    {
                        new Hotel
                        {
                            Name = string.Format("{0} Hilton", request.City),
                            NumberOfPeople = request.CustomerInfo.NumberOfAdults,
                        },
                        new Hotel
                        {
                            Name = string.Format("Best Western {0}", request.City),
                            NumberOfPeople = request.CustomerInfo.NumberOfAdults,
                        },
                        new Hotel
                        {
                            Name = string.Format("{0} Marriott Hotel", request.City),
                            NumberOfPeople = request.CustomerInfo.NumberOfAdults,
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                return new HotelResponse
                {
                    Status = new Status
                    {
                        Error =
                            "Something went terribly wrong!"
                    }
                };
            }
        }
    }

    [DataContract(Namespace = "http://api.luxtripper.com/LoginResponse")]
    public class LoginResponse
    {
        [DataMember(IsRequired = true)]
        public Status Status { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [XmlAttribute(Namespace = "http://api.luxtripper.com/LoginResponse/TOKEN_MAGIC")]
        [DataMember]
        public string Token { get; set; }
    }

    [DataContract(Namespace = "http://api.luxtripper.com/LoginRequest")]
    public class LoginRequest
    {
        [DataMember(Order = 1)]
        public string UserName { get; set; }
        [DataMember(Order = 2)]
        public string Password { get; set; }
    }

    [DataContract(Namespace = "http://api.luxtripper.com/HotelRequest")]
    public class HotelRequest
    {
        [DataMember]
        public Security Security { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public DateTime CheckInDate { get; set; }
        [DataMember]
        public DateTime CheckOutDate { get; set; }
        [DataMember]
        public CustomerInfo CustomerInfo { get; set; }
    }

    public class Security
    {
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Token { get; set; }
    }

    [DataContract(Namespace = "http://api.luxtripper.com/CustomerInfo")]
    public class CustomerInfo
    {
        [DataMember]
        public int NumberOfAdults { get; set; }
    }

    [DataContract(Namespace = "http://api.luxtripper.com/HotelResponse")]
    public class HotelResponse
    {
        [DataMember(IsRequired = true, Order = 1)]
        public Status Status { get; set; }
        [DataMember(IsRequired = false, Order = 2)]
        public Hotel[] Hotels { get; set; }
    }

    [DataContract(Namespace = "http://api.luxtripper.com/Hotel")]
    public class Hotel
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int NumberOfPeople { get; set; }
    }

    [DataContract(Namespace = "http://api.luxtripper.com/Status")]
    public class Status
    {
        [DataMember(IsRequired = true, Order = 1)]
        public bool Success { get; set; }
        [DataMember(IsRequired = false, Order = 2)]
        public string Error { get; set; }
    }
}
