var deadTime : float;

function Awake () {
if (deadTime > 0)
	Destroy (gameObject, deadTime);
}