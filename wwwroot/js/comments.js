function enableEdit(commentId) {
    console.log("Edytowanie komentarza:", commentId); // Debug
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
    console.log("Anulowanie edycji:", commentId); // Debug
    const displayDiv = document.getElementById(`comment-display-${commentId}`);
    const editDiv = document.getElementById(`comment-edit-${commentId}`);
    if (displayDiv && editDiv) {
        displayDiv.style.display = "block";
        editDiv.style.display = "none";
    } else {
        console.error("Nie znaleziono elementów do anulowania edycji:", commentId);
    }
}
