let deleteImageBtns = document.querySelectorAll(".delete-image-btn")

deleteImageBtns.forEach(btn => btn.addEventListener("click", function () {
    btn.parentElement.remove()
}))