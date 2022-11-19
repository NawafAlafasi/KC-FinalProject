var player = document.getElementById("player");
var object = document.getElementById("object");
let score = document.getElementById("score");
score.innerHTML = "";
let counter = 0

function jump(){
    if(player.classList != "animate"){
        player.classList.add("animate");
    }
    player.classList.add("animate");
    setTimeout(function(){player.classList.remove("animate");},500)
}

var checkDead = setInterval(function(){
    var playerTOP = parseInt(window.getComputedStyle(player).getPropertyValue("top"));
    var objectLeft = parseInt(window.getComputedStyle(object).getPropertyValue("left"));
    if(objectLeft<20 && objectLeft>0 && playerTOP>=130){
        object.style.animation = "none";
        object.style.display = "none";
        alert("u lose.");
        score.style.display = "none";
    }
    else if (objectLeft<0){
        counter++
        document.getElementById("score").innerHTML = counter
    }
},10)