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
        //Employees
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
        CarsReturnMarkDesc
    }

    public class SortViewModel
    {
        //CarMark
        public SortState CarMarkNameSort { get; set; }
        //Employees
        public SortState EmployeesPostSort { get; set; }
        public SortState EmployeesNameSort { get; set; }
        public SortState EmployeesSurnameSort { get; set; }
        public SortState EmployeesPatronymicSort { get; set; }
        public SortState EmployeesEmploymentDateSort { get; set; }
        //CarModels
        public SortState CarModelsNameSort { get; set; }
        public SortState CarModelsDescriptionSort { get; set; }
        //Cars
        public SortState CarsRegNum { get; set; }
        public SortState CarsVINcode { get; set; }
        public SortState CarsEngineNum { get; set; }
        public SortState CarsPrice { get; set; }
        public SortState CarsRentalPrice { get; set; }
        public SortState CarsIssueDate { get; set; }
        public SortState CarsSpecs { get; set; }
        public SortState CarsTechnicalMaintenanceDate { get; set; }
        public SortState CarsSpecMark { get; set; }
        public SortState CarsReturnMark { get; set; }


        public SortState CurrentState { get; set; }
        public SortViewModel(SortState state)
        {
            //CarMark
            CarMarkNameSort = state == SortState.CarMarkNameAsc ? SortState.CarMarkNameDesc : SortState.CarMarkNameAsc;
            CurrentState = state;
            //Employees
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

            CarsRegNum = state == SortState.CarsRegNumAsc ? SortState.CarsRegNumDesc : SortState.CarsRegNumAsc;
            CurrentState = state;

            CarsVINcode = state == SortState.CarsVINcodeAsc ? SortState.CarsVINcodeDesc : SortState.CarsVINcodeAsc;
            CurrentState = state;

            CarsEngineNum = state == SortState.CarsEngineNumAsc ? SortState.CarsEngineNumDesc : SortState.CarsEngineNumAsc;
            CurrentState = state;

            CarsPrice = state == SortState.CarsPriceAsc ? SortState.CarsPriceDesc : SortState.CarsPriceAsc;
            CurrentState = state;

            CarsRentalPrice = state == SortState.CarsRentalPriceAsc ? SortState.CarsRentalPriceDesc : SortState.CarsRentalPriceAsc;
            CurrentState = state;

            CarsIssueDate = state == SortState.CarsIssueDateAsc ? SortState.CarsIssueDateDesc : SortState.CarsIssueDateAsc;
            CurrentState = state;

            CarsSpecs = state == SortState.CarsSpecsAsc ? SortState.CarsSpecsDesc : SortState.CarsSpecsAsc;
            CurrentState = state;

            CarsTechnicalMaintenanceDate = state == SortState.CarsTechnicalMaintenanceDateAsc ? SortState.CarsTechnicalMaintenanceDateDesc : SortState.CarsTechnicalMaintenanceDateAsc;
            CurrentState = state;

            CarsSpecMark = state == SortState.CarsSpecMarkAsc ? SortState.CarsSpecMarkDesc : SortState.CarsSpecMarkAsc;
            CurrentState = state;

            CarsReturnMark = state == SortState.CarsReturnMarkAsc ? SortState.CarsReturnMarkDesc : SortState.CarsReturnMarkAsc;
            CurrentState = state;
        }
    }
}
