function insertAtCursor(id, myValue) {
    var myField = document.getElementById(id);

    if (myField.selectionStart || myField.selectionStart == '0') {
	
        var startPos = myField.selectionStart;
        var endPos = myField.selectionEnd;
		
        myField.value = myField.value.substring(0, startPos)
            + myValue
            + myField.value.substring(endPos, myField.value.length);
			
    } else {
        myField.value += myValue;
    }
}

function getSelectedText(id) {
    var selectedText = '';

    var myTextAera = document.getElementById(id);
    var startPos = myTextAera.selectionStart;
    var endPos = myTextAera.selectionEnd;
    var selectedText = myTextAera.value.substring(startPos, endPos);

    return selectedText;
}