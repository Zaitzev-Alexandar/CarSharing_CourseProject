using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSharing.ViewModels
{
    public enum SortState
    {
        No, 
        //CarMark
        CarMarkNameAsc,
        CarMarkNameDesc,
        //employees
        EmployeesPostAsc,
        EmployeesNameAsc,
        EmployeesSurnameAsc,
        EmployeesPatronymicAsc,
        EmployeesEmploymentDateAsc,
        EmployeesPostDesc,
        EmployeesNameDesc,
        EmployeesSurnameDesc,
        EmployeesPatronymicDesc,
        EmployeesEmploymentDateDesc,
        //CarModels
        CarModelsNameAsc,
        CarModelsDescriptionAsc,
        CarModelsNameDesc,
        CarModelsDescriptionDesc,
        //Cars
        CarsRegNumAsc,
        CarsVINcodeAsc,
        CarsEngineNumAsc,
        CarsPriceAsc,
        CarsRentalPriceAsc,
        CarsIssueDateAsc,
        CarsSpecsAsc,
        CarsTechnicalMaintenanceDateAsc,
        CarsSpecMarkAsc,
        CarsReturnMarkAsc,

        CarsRegNumDesc,
        CarsVINcodeDesc,
        CarsEngineNumDesc,
        CarsPriceDesc,
        CarsRentalPriceDesc,
        CarsIssueDateDesc,
        CarsSpecsDesc,
        CarsTechnicalMaintenanceDateDesc,
        CarsSpecMarkDesc,
        CarsReturnMarkDesc,
        //Services
        ServicesNameAsc,
        ServicesNameDesc,
        ServicesPriceAsc,
        ServicesPriceDesc,
        ServicesDescriptionAsc,
        ServicesDescriptionDesc,
        //Customers
        CustomersNameAsc,
        CustomersSurnameAsc,
        CustomersPatronymicAsc,
        CustomersPhoneNumAsc,
        CustomersAddressAsc,
        CustomersBirthDateAsc,
        CustomersPassportInfoAsc,
        CustomersGenderAsc,

        CustomersNameDesc,
        CustomersSurnameDesc,
        CustomersPatronymicDesc,
        CustomersPhoneNumDesc,
        CustomersAddressDesc,
        CustomersBirthDateDesc,
        CustomersPassportInfoDesc,
        CustomersGenderDesc,
        //AdditionalServices
        AdditionalServiceServiceNameAsc,
        AdditionalServiceServiceNameDesc,
        AdditionalServiceRentIdAsc,
        AdditionalServiceRentIdDesc,
        //Rents
        RentReturnDateAsc,
        RentReturnDateDesc,
        RentDeliveryDateAsc,
        RentDeliveryDateDesc,
        RentPriceAsc,
        RentPriceDesc
        
    }

    public class SortViewModel
    {
        //Rents
        public SortState RentReturnDateSort { get; set; }
        public SortState RentDeliveryDateSort { get; set; }
        public SortState RentPriceSort { get; set; }
        //AdditionalServices
        public SortState AdditionalServiceServiceNameSort { get; set; }
        public SortState AdditionalServiceRentIdSort { get; set; }

        //CarMark
        public SortState CarMarkNameSort { get; set; }
        //employees
        public SortState EmployeesPostSort { get; set; }
        public SortState EmployeesNameSort { get; set; }
        public SortState EmployeesSurnameSort { get; set; }
        public SortState EmployeesPatronymicSort { get; set; }
        public SortState EmployeesEmploymentDateSort { get; set; }
        //CarModels
        public SortState CarModelsNameSort { get; set; }
        public SortState CarModelsDescriptionSort { get; set; }
        //Cars
        public SortState CarsRegNumSort { get; set; }
        public SortState CarsVINcodeSort { get; set; }
        public SortState CarsEngineNumSort { get; set; }
        public SortState CarsPriceSort { get; set; }
        public SortState CarsRentalPriceSort { get; set; }
        public SortState CarsIssueDateSort { get; set; }
        public SortState CarsSpecsSort { get; set; }
        public SortState CarsTechnicalMaintenanceDateSort { get; set; }
        public SortState CarsSpecMarkSort { get; set; }
        public SortState CarsReturnMarkSort { get; set; }

        //Services
        public SortState ServicesNameSort { get; set; }
        public SortState ServicesPriceSort { get; set; }
        public SortState ServicesDescriptionSort { get; set; }

        //Customers
        public SortState CustomersNameSort { get; set; }
        public SortState CustomersSurnameSort { get; set; }
        public SortState CustomersPatronymicSort { get; set; }
        public SortState CustomersPhoneNumSort { get; set; }
        public SortState CustomersAddressSort { get; set; }
        public SortState CustomersBirthDateSort { get; set; }
        public SortState CustomersPassportInfoSort { get; set; }
        public SortState CustomersGenderSort { get; set; }


        public SortState CurrentState { get; set; }
        public SortViewModel(SortState state)
        {
            //Rents
            RentReturnDateSort = state == SortState.RentReturnDateAsc ? SortState.RentReturnDateDesc : SortState.RentReturnDateAsc;
            CurrentState = state;
           
            RentDeliveryDateSort = state == SortState.RentDeliveryDateAsc ? SortState.RentDeliveryDateDesc : SortState.RentDeliveryDateAsc;
            CurrentState = state;
            
            RentPriceSort = state == SortState.RentPriceAsc ? SortState.RentPriceDesc : SortState.RentPriceAsc;
            CurrentState = state;

            //AdditionalServices
            AdditionalServiceRentIdSort = state == SortState.AdditionalServiceRentIdAsc ? SortState.AdditionalServiceRentIdDesc : SortState.AdditionalServiceRentIdAsc;
            CurrentState = state;
            AdditionalServiceServiceNameSort = state == SortState.AdditionalServiceServiceNameAsc ? SortState.AdditionalServiceServiceNameDesc : SortState.AdditionalServiceServiceNameAsc;
            CurrentState = state;


            //Customers
            CustomersNameSort = state == SortState.CustomersNameAsc ? SortState.CustomersNameDesc : SortState.CustomersNameAsc;
            CurrentState = state;
            CustomersSurnameSort = state == SortState.CustomersSurnameAsc ? SortState.CustomersSurnameDesc : SortState.CustomersSurnameAsc;
            CurrentState = state;
            CustomersPatronymicSort = state == SortState.CustomersPatronymicAsc ? SortState.CustomersPatronymicDesc : SortState.CustomersPatronymicAsc;
            CurrentState = state;
            CustomersPhoneNumSort = state == SortState.CustomersPhoneNumAsc ? SortState.CustomersPhoneNumDesc : SortState.CustomersPhoneNumAsc;
            CurrentState = state;
            CustomersAddressSort = state == SortState.CustomersAddressAsc ? SortState.CustomersAddressDesc : SortState.CustomersAddressAsc;
            CurrentState = state;
            CustomersBirthDateSort = state == SortState.CustomersBirthDateAsc ? SortState.CustomersBirthDateDesc : SortState.CustomersBirthDateAsc;
            CurrentState = state;
            CustomersPassportInfoSort = state == SortState.CustomersPassportInfoAsc ? SortState.CustomersPassportInfoDesc : SortState.CustomersPassportInfoAsc;
            CurrentState = state;
            CustomersGenderSort = state == SortState.CustomersGenderAsc ? SortState.CustomersGenderDesc : SortState.CustomersGenderAsc;
            CurrentState = state;


            //Service
            ServicesNameSort = state == SortState.ServicesNameAsc ? SortState.ServicesNameDesc : SortState.ServicesNameAsc;
            CurrentState = state;
            ServicesPriceSort = state == SortState.ServicesPriceAsc ? SortState.ServicesPriceDesc : SortState.ServicesPriceAsc;
            CurrentState = state;
            ServicesDescriptionSort = state == SortState.ServicesDescriptionAsc ? SortState.ServicesDescriptionDesc : SortState.ServicesDescriptionAsc;
            CurrentState = state;



            //CarMark
            CarMarkNameSort = state == SortState.CarMarkNameAsc ? SortState.CarMarkNameDesc : SortState.CarMarkNameAsc;
            CurrentState = state;
            //employees
            EmployeesPostSort = state == SortState.EmployeesPostAsc ? SortState.EmployeesPostDesc : SortState.EmployeesPostAsc;
            CurrentState = state;

            EmployeesNameSort = state == SortState.EmployeesNameAsc ? SortState.EmployeesNameDesc : SortState.EmployeesNameAsc;
            CurrentState = state;

            EmployeesSurnameSort = state == SortState.EmployeesSurnameAsc ? SortState.EmployeesSurnameDesc : SortState.EmployeesSurnameAsc;
            CurrentState = state;

            EmployeesPatronymicSort = state == SortState.EmployeesPatronymicAsc ? SortState.EmployeesPatronymicDesc : SortState.EmployeesPatronymicAsc;
            CurrentState = state;

            EmployeesEmploymentDateSort = state == SortState.EmployeesEmploymentDateAsc ? SortState.EmployeesEmploymentDateDesc : SortState.EmployeesEmploymentDateAsc;
            CurrentState = state;
            //CarModels
            CarModelsNameSort = state == SortState.CarModelsNameAsc ? SortState.CarModelsNameDesc : SortState.CarModelsNameAsc;
            CurrentState = state;

            CarModelsDescriptionSort = state == SortState.CarModelsDescriptionAsc ? SortState.CarModelsDescriptionDesc : SortState.CarModelsDescriptionAsc;
            CurrentState = state;
            //Cars

            CarsRegNumSort = state == SortState.CarsRegNumAsc ? SortState.CarsRegNumDesc : SortState.CarsRegNumAsc;
            CurrentState = state;

            CarsVINcodeSort = state == SortState.CarsVINcodeAsc ? SortState.CarsVINcodeDesc : SortState.CarsVINcodeAsc;
            CurrentState = state;

            CarsEngineNumSort = state == SortState.CarsEngineNumAsc ? SortState.CarsEngineNumDesc : SortState.CarsEngineNumAsc;
            CurrentState = state;

            CarsPriceSort = state == SortState.CarsPriceAsc ? SortState.CarsPriceDesc : SortState.CarsPriceAsc;
            CurrentState = state;

            CarsRentalPriceSort = state == SortState.CarsRentalPriceAsc ? SortState.CarsRentalPriceDesc : SortState.CarsRentalPriceAsc;
            CurrentState = state;

            CarsIssueDateSort = state == SortState.CarsIssueDateAsc ? SortState.CarsIssueDateDesc : SortState.CarsIssueDateAsc;
            CurrentState = state;

            CarsSpecsSort = state == SortState.CarsSpecsAsc ? SortState.CarsSpecsDesc : SortState.CarsSpecsAsc;
            CurrentState = state;

            CarsTechnicalMaintenanceDateSort = state == SortState.CarsTechnicalMaintenanceDateAsc ? SortState.CarsTechnicalMaintenanceDateDesc : SortState.CarsTechnicalMaintenanceDateAsc;
            CurrentState = state;

            CarsSpecMarkSort = state == SortState.CarsSpecMarkAsc ? SortState.CarsSpecMarkDesc : SortState.CarsSpecMarkAsc;
            CurrentState = state;

            CarsReturnMarkSort = state == SortState.CarsReturnMarkAsc ? SortState.CarsReturnMarkDesc : SortState.CarsReturnMarkAsc;
            CurrentState = state;
        }
    }
}
