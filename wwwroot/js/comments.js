function validateComment(commentId) {
    const content = document.getElementById(`edit-content-${commentId}`).value.trim();
    const errorMessage = document.getElementById(`error-message-${commentId}`);

    if (content === "") {
        errorMessage.style.display = "block";
        return false;
    } else {
        errorMessage.style.display = "none";
        return true; 
    }
}

function enableEdit(commentId) {
    console.log("Edytowanie komentarza:", commentId); 
    const displayDiv = document.getElementById(`comment-display-${commentId}`);
    const editDiv = document.getElementById(`comment-edit-${commentId}`);
    if (displayDiv && editDiv) {
        displayDiv.style.display = "none";
        editDiv.style.display = "block";
    } else {
        console.error("Nie znaleziono elementów do edycji komentarza:", commentId);
    }
}

function cancelEdit(commentId) {
    console.log("Anulowanie edycji:", commentId);
    const displayDiv = document.getElementById(`comment-display-${commentId}`);
    const editDiv = document.getElementById(`comment-edit-${commentId}`);
    if (displayDiv && editDiv) {
        displayDiv.style.display = "block";
        editDiv.style.display = "none";
    } else {
        console.error("Nie znaleziono elementów do anulowania edycji:", commentId);
    }
}
