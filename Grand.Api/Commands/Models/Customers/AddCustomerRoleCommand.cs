using Grand.Api.DTOs.Customers;
using MediatR;

namespace Grand.Api.Commands.Models.Customers
{
    public class AddCustomerRoleCommand : IRequest<CustomerRoleDto>
    {
        public CustomerRoleDto Model { get; set; }
    }
}
