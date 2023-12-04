var isItemSelected = false;

function ItemSelected(source, eventArgs) {

    var idx = source._selectIndex;
    var items = source.get_completionList().childNodes;
    var value = items[idx]._value;
    //alert(value);
    var text = items[idx].firstChild.nodeValue;
    var strId = source._id;
    var btnPostBack = source._id.replace("AutoCompleteExtender1", "btnPostBack");
    var hdnSelectedValue = source._id.replace("AutoCompleteExtender1", "hdnSelectedValue");
    document.getElementById(hdnSelectedValue).value = value;
    var obj = document.getElementById(btnPostBack);
    __doPostBack(obj.name, "OnClick");
    source.get_element().value = text;
    isItemSelected = true;
    var txtDropDown = source._id.replace("AutoCompleteExtender1", "txtDropDown");
    document.getElementById(txtDropDown).focus();
  
}

function checkItemSelected(txtInput) {

    if (txtInput.value == '') {
        if (isItemSelected == true)
            isItemSelected = false;
    }
    else {
        if (!isItemSelected) {
            //bootbox.alert("PLEASE SELECT ITEM FROM LIST ONLY");
        }
        else {
            isItemSelected = false;
        }
    }

}

function resetItemSelected() {
    isItemSelected = false;
}
 

function OnClientPopulating(sender, e) {
    sender._element.className = "loading form-control";
}
function OnClientCompleted(sender, e) {
    sender._element.className = "form-control";
}

