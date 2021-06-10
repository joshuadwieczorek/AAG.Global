using System;
using System.Collections.Generic;
using System.Xml;

namespace AAG.Global.Utilities
{
    public sealed class AdfGeneratorUtility
    {
        private Object ThreadLock { get; set; }
        private XmlDocument Document { get; set; }
        private string FullName { get; set; }
        private string EmailAddress { get; set; }
        private string PhoneNumber { get; set; }
        private string VehicleCondition { get; set; }
        private int? VehicleYear { get; set; }
        private string VehicleMake { get; set; }
        private string VehicleModel { get; set; }
        private string VehicleStockNumber { get; set; }
        private string VehicleVinNumber { get; set; }
        private string LeadId { get; set; }
        private string LeadSource { get; set; }
        private string Comments { get; set; }
        private string VendorName { get; set; }
        private string VendorContact { get; set; }
        private string ProviderName { get; set; }
        private string ProviderContact { get; set; }
        private string ProviderService { get; set; }
        private string ProviderUrl { get; set; }
        private string ProviderEmail { get; set; }
        private string ProviderPhone { get; set; }
        private DateTime? RequestDate { get; set; }
        private string Address1 { get; set; }
        private string Address2 { get; set; }
        private string City { get; set; }
        private string State { get; set; }
        private int? Zip { get; set; }
        private string Country { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public AdfGeneratorUtility()
        {
            ThreadLock = new Object();
        }


        /// <summary>
        /// Set full name.
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetFullName(string fullName)
        {
            FullName = fullName;
            return this;
        }


        /// <summary>
        /// Override the Tostring method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("\n", "<?adf version \"1.0\"?>", Generate().OuterXml);
        }


        /// <summary>
        /// Generate xml document.
        /// </summary>
        /// <returns></returns>
        public XmlDocument Generate()
        {
            lock (ThreadLock)
            {
                Document = new XmlDocument();
                XmlDeclaration declaration = Document.CreateXmlDeclaration("1.0", "UTF-16", null);
                XmlElement root = Document.DocumentElement;
                Document.InsertBefore(declaration, root);
                XmlElement adf = Document.CreateElement(string.Empty, "adf", string.Empty);
                Document.AppendChild(adf);
                XmlElement prospect = Document.CreateElement(string.Empty, "prospect", string.Empty);
                CreateSimpleXmlElement(ref prospect, "id", LeadId, new KeyValuePair<string, string>("sequence", "1"), new KeyValuePair<string, string>("source", LeadSource));
                CreateSimpleXmlElement(ref prospect, "requestdate", (RequestDate.HasValue ? RequestDate.Value.ToString("yyyy-MM-ddTHH:mm:ss") : DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")));
                CreateCustomerContactElements(ref prospect);
                CreateVehicleElements(ref prospect);
                CreateVendorElements(ref prospect);
                CreateProviderElements(ref prospect);
                adf.AppendChild(prospect);
                return Document;
            }
        }

        #region "Document parts creators"
        /// <summary>
        /// Create customer & contact elements.
        /// </summary>
        /// <param name="prospect"></param>
        private void CreateCustomerContactElements(ref XmlElement prospect)
        {
            XmlElement customer = Document.CreateElement("customer");
            XmlElement contact = Document.CreateElement("contact");
            CreateSimpleXmlElement(ref contact, "name", FullName, new KeyValuePair<string, string>("part", "full")); CreateSimpleXmlElement(ref contact, "email", EmailAddress);
            CreateSimpleXmlElement(ref contact, "phone", PhoneNumber, new KeyValuePair<string, string>("type", "voice"));
            CreateCustomerContactAddressElements(ref contact);
            CreateSimpleXmlElement(ref contact, "comments", Comments);
            customer.AppendChild(contact);
            prospect.AppendChild(customer);
        }


        /// <summary>
        /// Create customer contact address elements.
        /// </summary>
        /// <param name="contact"></param>
        private void CreateCustomerContactAddressElements(ref XmlElement contact)
        {
            XmlElement address = Document.CreateElement("address");
            CreateSimpleXmlElement(ref address, "street", Address1, new KeyValuePair<string, string>("line", "1"));
            CreateSimpleXmlElement(ref address, "apartment", Address2);
            CreateSimpleXmlElement(ref address, "city", City);
            CreateSimpleXmlElement(ref address, "regioncode", State);
            if (Zip.HasValue)
                CreateSimpleXmlElement(ref address, "postalcode", Zip.Value.ToString());
            else
                CreateSimpleXmlElement(ref address, "postalcode", null);
            CreateSimpleXmlElement(ref address, "country", Country);
        }


        /// <summary>
        /// Create vehicle elements.
        /// </summary>
        /// <param name="prospect"></param>
        private void CreateVehicleElements(ref XmlElement prospect)
        {
            XmlElement vehicle = Document.CreateElement("vehicle");

            if (!string.IsNullOrEmpty(VehicleCondition))
                vehicle.SetAttribute("status", "used");

            if (VehicleYear.HasValue)
                CreateSimpleXmlElement(ref vehicle, "year", VehicleYear.Value.ToString());
            else
                CreateSimpleXmlElement(ref vehicle, "year", null);
            CreateSimpleXmlElement(ref vehicle, "make", VehicleMake);
            CreateSimpleXmlElement(ref vehicle, "model", VehicleModel);
            CreateSimpleXmlElement(ref vehicle, "stock", VehicleStockNumber);
            CreateSimpleXmlElement(ref vehicle, "vin", VehicleVinNumber);
            prospect.AppendChild(vehicle);
        }


        /// <summary>
        /// Create vendor elements.
        /// </summary>
        /// <param name="prospect"></param>
        private void CreateVendorElements(ref XmlElement prospect)
        {
            XmlElement vendor = Document.CreateElement("vendor");
            XmlElement contact = Document.CreateElement("contact");
            CreateSimpleXmlElement(ref vendor, "vendorname", VendorName);
            CreateSimpleXmlElement(ref contact, "name", VendorContact, new KeyValuePair<string, string>("part", "full"));
            vendor.AppendChild(contact);
            prospect.AppendChild(vendor);
        }


        /// <summary>
        /// Create provider elements.
        /// </summary>
        /// <param name="prospect"></param>
        private void CreateProviderElements(ref XmlElement prospect)
        {
            XmlElement provider = Document.CreateElement("provider");
            XmlElement contact = Document.CreateElement("contact");
            XmlElement address = Document.CreateElement("address");
            CreateSimpleXmlElement(ref provider, "id", LeadId, new KeyValuePair<string, string>("source", ProviderName));
            CreateSimpleXmlElement(ref provider, "name", ProviderName, new KeyValuePair<string, string>("part", "full"));
            CreateSimpleXmlElement(ref provider, "service", ProviderService);
            CreateSimpleXmlElement(ref provider, "url", ProviderUrl);
            CreateSimpleXmlElement(ref provider, "email", ProviderEmail);
            CreateSimpleXmlElement(ref provider, "phone", ProviderPhone);
            CreateSimpleXmlElement(ref contact, "name", ProviderContact, new KeyValuePair<string, string>("part", "full"));
            CreateSimpleXmlElement(ref contact, "email", "");
            CreateSimpleXmlElement(ref contact, "phone", "");
            CreateSimpleXmlElement(ref address, "street", "", new KeyValuePair<string, string>("line", "1"));
            CreateSimpleXmlElement(ref address, "street", "", new KeyValuePair<string, string>("line", "2"));
            CreateSimpleXmlElement(ref address, "city", "");
            CreateSimpleXmlElement(ref address, "regioncode", "FL");
            CreateSimpleXmlElement(ref address, "postalcode", "");
            CreateSimpleXmlElement(ref address, "country", "US");
            contact.AppendChild(address);
            provider.AppendChild(contact);
            prospect.AppendChild(provider);
        }


        /// <summary>
        /// Create XML Element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="attributes"></param>
        private void CreateSimpleXmlElement(ref XmlElement element, string name, string value, params KeyValuePair<string, string>[] attributes)
        {
            if (element != null && !string.IsNullOrEmpty(name))
            {
                XmlElement newElement = Document.CreateElement(name);

                if (!string.IsNullOrEmpty(value))
                {
                    XmlText newValue = Document.CreateTextNode(value);
                    newElement.AppendChild(newValue);
                }

                if (attributes != null && attributes.Length > 0)
                {
                    foreach (KeyValuePair<string, string> attribute in attributes)
                        newElement.SetAttribute(attribute.Key, attribute.Value);
                }

                element.AppendChild(newElement);
            }
        }
        #endregion "Document parts creators"


        #region "Setter methods"
        /// <summary>
        /// Set full name.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetFullName(string firstName, string lastName)
        {
            FullName = string.Join(" ", firstName, lastName);
            return this;
        }


        /// <summary>
        /// Set email address.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetEmailAddress(string emailAddress)
        {
            EmailAddress = emailAddress;
            return this;
        }


        /// <summary>
        /// Set phone number.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetPhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
            return this;
        }


        /// <summary>
        /// Set address1.
        /// </summary>
        /// <param name="address1"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetAddress1(string address1)
        {
            Address1 = address1;
            return this;
        }


        /// <summary>
        /// Set address2.
        /// </summary>
        /// <param name="address2"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetAddress2(string address2)
        {
            Address2 = address2;
            return this;
        }


        /// <summary>
        /// Set city.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetCity(string city)
        {
            City = city;
            return this;
        }


        /// <summary>
        /// Set state.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetState(string state)
        {
            State = state;
            return this;
        }


        /// <summary>
        /// Set zip.
        /// </summary>
        /// <param name="zip"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetZip(int zip)
        {
            Zip = zip;
            return this;
        }


        /// <summary>
        /// Set country.
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetCountry(string country)
        {
            Country = country;
            return this;
        }

        /// <summary>
        /// Set vehicle condition.
        /// </summary>
        /// <param name="vehicleCondition"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetVehicleCondition(string vehicleCondition)
        {
            VehicleCondition = vehicleCondition;
            return this;
        }

        /// <summary>
        /// Set vehicle year.
        /// </summary>
        /// <param name="vehicleYear"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetVehicleYear(int vehicleYear)
        {
            VehicleYear = vehicleYear;
            return this;
        }


        /// <summary>
        /// Set vehicle make.
        /// </summary>
        /// <param name="vehicleMake"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetVehicleMake(string vehicleMake)
        {
            VehicleMake = vehicleMake;
            return this;
        }


        /// <summary>
        /// Set vehicle model.
        /// </summary>
        /// <param name="vehicleModel"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetVehicleModel(string vehicleModel)
        {
            VehicleModel = vehicleModel;
            return this;
        }


        /// <summary>
        /// Set vehicle stock number.
        /// </summary>
        /// <param name="vehicleStockNumber"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetVehicleStockNumber(string vehicleStockNumber)
        {
            VehicleStockNumber = vehicleStockNumber;
            return this;
        }


        /// <summary>
        /// Set vehicle vin number.
        /// </summary>
        /// <param name="vehicleVinNumber"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetVehicleVinNumber(string vehicleVinNumber)
        {
            VehicleVinNumber = vehicleVinNumber;
            return this;
        }


        /// <summary>
        /// Set lead id.
        /// </summary>
        /// <param name="leadId"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetLeadId(string leadId)
        {
            LeadId = leadId;
            return this;
        }


        /// <summary>
        /// Set lead source.
        /// </summary>
        /// <param name="leadSource"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetLeadSource(string leadSource)
        {
            LeadSource = leadSource;
            return this;
        }


        /// <summary>
        /// Set vendor name.
        /// </summary>
        /// <param name="vendorName"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetVendorName(string vendorName)
        {
            VendorName = vendorName;
            return this;
        }


        /// <summary>
        /// Set vendor contact.
        /// </summary>
        /// <param name="vendorContact"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetVendorContact(string vendorContact)
        {
            VendorContact = vendorContact;
            return this;
        }


        /// <summary>
        /// Set provider name.
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetProviderName(string providerName)
        {
            ProviderName = providerName;
            return this;
        }


        /// <summary>
        /// Set provider contact.
        /// </summary>
        /// <param name="providerContact"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetProviderContact(string providerContact)
        {
            ProviderContact = providerContact;
            return this;
        }


        /// <summary>
        /// Set provider contact.
        /// </summary>
        /// <param name="providerService"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetProviderService(string providerService)
        {
            ProviderService = providerService;
            return this;
        }


        /// <summary>
        /// Set provider url.
        /// </summary>
        /// <param name="providerUrl"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetProviderUrl(string providerUrl)
        {
            ProviderUrl = providerUrl;
            return this;
        }


        /// <summary>
        /// Set provider email.
        /// </summary>
        /// <param name="providerEmail"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetProviderEmail(string providerEmail)
        {
            ProviderEmail = providerEmail;
            return this;
        }


        /// <summary>
        /// Set provider phone.
        /// </summary>
        /// <param name="providerPhone"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetProviderPhone(string providerPhone)
        {
            ProviderPhone = providerPhone;
            return this;
        }


        /// <summary>
        /// Set comments.
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetComments(string comments)
        {
            Comments = comments;
            return this;
        }


        /// <summary>
        /// Set request date.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public AdfGeneratorUtility SetRequestDate(DateTime? dateTime = null)
        {
            RequestDate = dateTime ?? DateTime.Now;
            return this;
        }
        #endregion "Setter methods"
    }
}