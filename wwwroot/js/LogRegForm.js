    document.addEventListener("DOMContentLoaded", function (event) {
        if (sessionStorage) {
            if (loginBtn && signupBtn && loginForm && loginText) {
                if (!sessionStorage.getItem('margin') || sessionStorage.getItem('margin') === '0') {
                    loginBtn.click();
                } else {
                    signupBtn.click();
                }
            }
        }
    });

    const loginText = document.querySelector(".title-text .login");
    const loginForm = document.querySelector("form.login");
    const loginBtn = document.querySelector("label.login");
    const signupBtn = document.querySelector("label.signup");
    const signupLink = document.querySelector("form .signup-link a");

    signupBtn.onclick = () => {
        if (sessionStorage) {
            sessionStorage.setItem('margin', '1');
            loginForm.style.marginLeft = "-50%";
            loginText.style.marginLeft = "-50%";
        }
    };

    loginBtn.onclick = () => {
        if (sessionStorage) {
            sessionStorage.setItem('margin', '0');
            loginForm.style.marginLeft = "0%";
            loginText.style.marginLeft = "0%";
        }
    };