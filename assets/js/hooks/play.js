
import {Player} from "../game/player.js"

export const Play = function () {
    
    this.mounted = function () {
        let player = new Player(1)

        document.addEventListener("keypress", function onPress(event) {
            if (event.key === "a") {
                player.move("left")
            }
            if (event.key === "w") {
                player.move("up")
            }
            if (event.key === "s") {
                player.move("down")
            }
            if (event.key === "d") {
                player.move("right")
            }
        });
    }
}


