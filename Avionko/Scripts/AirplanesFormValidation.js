
// a function that validates tickets form
//(user has to fill in all needed data )
function ValidateAirTicketForm()
{

    var passengerNumber = document.getElementById("passengerNumber").value;
    var currency = document.getElementById("currency").value;


    if (currency == "valuta")
    {
        $("#validationFailedTxt").fadeIn(1000, function () {
            // Animation complete.
        });
        document.getElementById("validationFailedTxt").innerHTML = "Niste odabrali valutu. Molimo odaberite valutu.";
        $("#validationFailedTxt").fadeOut(1500, function () {
            // Animation complete.
        });

        return false;
    }
    else if (passengerNumber == "broj putnika")
    {
        $("#validationFailedTxt").fadeIn(1000, function () {
            // Animation complete.
        });
        document.getElementById("validationFailedTxt").innerHTML = "Niste odabrali broj putnika. Molimo odaberite broj putnika.";
        $("#validationFailedTxt").fadeOut(1500, function () {
            // Animation complete.
        });
        return false;
    }
    else
    {
        document.getElementById("validationFailedTxt").innerHTML = "";
        return true;
    }

}