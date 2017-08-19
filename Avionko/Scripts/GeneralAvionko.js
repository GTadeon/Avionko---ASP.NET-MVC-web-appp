///script that contains some general methods for Avionko task 


///hides search results div (showing is done in backend, if form gets validated .
//call this when "ponovi pretragu" button is clicked or if user shouldn't be seeing search results div (backend check).
function HideSearchResultDiv()
{
    $("#searchResults").hide();
}