
$(document).ready(
    function () {
        hookToSearchControlEvents();   
    });

function hookToSearchControlEvents() {
    configAutoCompleteTags(onStartAutoCompleteSearch, onStartAutoCompleteSearch, '#headerSearchControl');
}

function onStartAutoCompleteSearch() {
    console.log("Search data: " + $('#headerSearchControl').val())
    //searchData = readSearchData();
    //searchData.searchText = $('#searchImageTag').val();
    //saveSearchData(searchData);
}