using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Xml.Serialization;

namespace Luxtripper.Api.Controllers
{
    public class HotelServiceController : ApiController
    {
        [HttpPost, Route("api/Hello")]
        public async Task<FormattedContentResult<HotelRequest>> Hello(LoginRequest request)
        {
            if (HttpContext.Current.Request.ContentType.Contains("json")) throw new Exception("No Json allowed, ContentType must be: text/xml");
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var response =
                new HotelRequest
                {
                    Security = new Security { UserName = "DiscoCat", Token = "A9-SDTES-NIC0-WR132-412422" },
                    City = "Stockholm",
                    CheckInDate = DateTime.Now,
                    CheckOutDate = DateTime.Now.AddDays(12),
                    CustomerInfo = new CustomerInfo { NumberOfAdults = 2 }
                };
            return Content(HttpStatusCode.OK, response,
                Configuration.Formatters.XmlFormatter);
        }


        [HttpPost, Route("api/HelloX")]
        public async Task<FormattedContentResult<LoginRequest>> HelloX(LoginRequest request)
        {
            if (HttpContext.Current.Request.ContentType.Contains("json")) throw new Exception("No Json allowed, ContentType must be: text/xml");
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var response =
                new LoginRequest
                {
                    UserName = "DiscoCat",
                    Password = "NinjaMoves"
                };
            return Content(HttpStatusCode.OK, response,
                Configuration.Formatters.XmlFormatter);
        }

        /// <summary>
        /// You first have to log in to get the token.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("api/Login")]
        public async Task<FormattedContentResult<LoginResponse>> Login(LoginRequest request)
        {
            if (HttpContext.Current.Request.ContentType.Contains("json")) throw new Exception("No Json allowed, ContentType must be: text/xml");
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            LoginResponse response;
            try
            {
                if (request.UserName != "DiscoCat" || request.Password != "NinjaMoves")
                {
                    response = new LoginResponse
                    {
                        Status = new Status
                        {
                            Error =
                                "Not Authorized: Your credentials are = UserName: DiscoCat, Password: NinjaMoves"
                        }
                    };
                    return Content(HttpStatusCode.OK, response,
                        Configuration.Formatters.XmlFormatter);
                }
                response = new LoginResponse
                {
                    Status = new Status { Success = true },
                    UserName = request.UserName,
                    Token = "A9-SDTES-NIC0-WR132-412422"
                };
                return Content(HttpStatusCode.OK, response,
                    Configuration.Formatters.XmlFormatter);
            }
            catch (Exception ex)
            {
                response =
                    new LoginResponse
                    {
                        Status = new Status
                        {
                            Error =
                                "Something went terribly wrong!"
                        }
                    };
                return Content(HttpStatusCode.OK, response,
                    Configuration.Formatters.XmlFormatter);
            }
        }

        /// <summary>
        /// When you've got the token from the Login Call then you are free to use the API.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FormattedContentResult<HotelResponse>> GetHotels(HotelRequest request)
        {
            if (HttpContext.Current.Request.ContentType.Contains("json")) throw new Exception("No Json allowed, ContentType must be: text/xml");
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            HotelResponse response;
            try
            {
                if (request.Security.Token == null)
                {
                    response = new HotelResponse
                    {
                        Status = new Status
                        {
                            Error =
                                "Missing Token: You first have to login with your credentials (UserName: DiscoCat, Password: NinjaMoves)"
                        }
                    };
                    return Content(HttpStatusCode.OK, response,
                        Configuration.Formatters.XmlFormatter);
                }
                if (request.Security.UserName != "DiscoCat" || request.Security.Token != "A9-SDTES-NIC0-WR132-412422")
                {
                    response = new HotelResponse
                    {
                        Status = new Status
                        {
                            Error =
                                "Illegal Token: You first have to login with your credentials (UserName: DiscoCat, Password: NinjaMoves)"
                        }
                    };
                    return Content(HttpStatusCode.OK, response,
                        Configuration.Formatters.XmlFormatter);
                }
                response = new HotelResponse
                {
                    Status = new Status { Success = true },
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
                return Content(HttpStatusCode.OK, response,
                    Configuration.Formatters.XmlFormatter);
            }
            catch (Exception ex)
            {
                response =
                    new HotelResponse
                    {
                        Status = new Status
                        {
                            Error =
                                "Something went terribly wrong!"
                        }
                    };
                return Content(HttpStatusCode.OK, response,
                    Configuration.Formatters.XmlFormatter);
            }
        }
    }

    [XmlRoot("login_response", Namespace = "http://api.luxtripper.com/LoginResponse")]
    public class LoginResponse
    {
        [XmlElement("status")]
        public Status Status { get; set; }
        [XmlElement("user_name")]
        public string UserName { get; set; }
        [XmlAttribute("token", Namespace = "http://api.luxtripper.com/LoginResponse/TOKEN_MAGIC")]
        public string Token { get; set; }
    }

    [XmlRoot("login_request", Namespace = "http://api.luxtripper.com/LoginRequest")]
    public class LoginRequest
    {
        [XmlElement("user_name")]
        public string UserName { get; set; }
        [XmlElement("password")]
        public string Password { get; set; }
    }

    [XmlRoot("hotel_request", Namespace = "http://api.luxtripper.com/HotelRequest")]
    public class HotelRequest
    {
        [XmlElement("security", Order = 1)]
        public Security Security { get; set; }
        [XmlElement("city", Order = 2)]
        public string City { get; set; }
        [XmlElement("check_in_date", Order = 3)]
        public DateTime CheckInDate { get; set; }
        [XmlElement("check_out_date", Order = 4)]
        public DateTime CheckOutDate { get; set; }
        [XmlElement("customer_info", Order = 5)]
        public CustomerInfo CustomerInfo { get; set; }
    }

    public class Security
    {
        [XmlElement("user_name", Order = 1)]
        public string UserName { get; set; }
        [XmlElement("token", Order = 2)]
        public string Token { get; set; }
    }

    public class CustomerInfo
    {
        [XmlElement("number_of_adults", Order = 1)]
        public int NumberOfAdults { get; set; }
    }

    [XmlRoot("hotel_response", Namespace = "http://api.luxtripper.com/HotelResponse")]
    public class HotelResponse
    {
        public HotelResponse() { }

        [XmlElement("status", Order = 1)]
        public Status Status { get; set; }

        [XmlArray("hotels", Order = 2)]
        [XmlArrayItem("hotel")]
        public Hotel[] Hotels { get; set; }
    }

    public class Hotel
    {
        [XmlAttribute("number_of_people")]
        public int NumberOfPeople { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
    }

    public class Status
    {
        [XmlAttribute("success")]
        public bool Success { get; set; }
        [XmlText]
        public string Error { get; set; }
    }
}
