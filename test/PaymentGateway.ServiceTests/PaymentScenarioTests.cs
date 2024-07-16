using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace PaymentGateway.ServiceTests;

public partial class PaymentScenarioTests : FeatureFixture
{
    [Scenario]
    public async Task PaymentGateway_process_payment_endpoint_success()
    {
        await Runner.RunScenarioAsync(given => User_with_valid_card_and_enough_balance(),
            when => Payment_is_attempted(),
            then => Response_is_authorized());
    }
    
    [Scenario]
    public async Task PaymentGateway_process_payment_endpoint_failed()
    {
        await Runner.RunScenarioAsync(given => User_with_valid_card_and_not_enough_balance(),
            when => Payment_is_attempted(),
            then => Response_is_declined());
    }
    
    [Scenario]
    public async Task PaymentGateway_process_payment_endpoint_rejected()
    {
        await Runner.RunScenarioAsync(given => User_with_invalid_card(),
            when => Payment_is_attempted(),
            then => Response_is_rejected());
    }
    
    [Scenario]
    public async Task PaymentGateway_get_payment_endpoint_success()
    {
        await Runner.RunScenarioAsync(given => User_made_a_payment(),
            when => Merchant_retrieves_the_payment(),
            then => Payment_response_is_provided());
    }

    [Scenario] public async Task PaymentGateway_get_payment_endpoint_failure()
    {
        await Runner.RunScenarioAsync(when => Merchant_retrieves_non_existing_payment(),
            then => Payment_response_is_provided_with_not_found());
    }
    
    
}