    function showToast(message) {
        Toastify({
            text: message,
            duration: 3000,
            close: true,
            gravity: "top",
            position: "right",
            backgroundColor: "#fbe593",
            className: "ponuda-toast",                
        }).showToast();
    }

export { showToast };