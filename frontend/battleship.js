var firstClick = false;
var firstRow;
var firstColumn;

var username;

var playertoken;

var shipPlacement = false;
var missileLaunch = false;

var shipTBP;
var indexOfshipTBP;

var placed = [];

var isTurn;

window.onload = function() {
	$("#update-game").click(getState);
	createGrid("battleship-body");
	createGrid("enemy-battleship-body");
	$("#submitToken").click(function(ev) {
		var tokenusername = $("#connect").serialize();
		$.ajax({
	        url: "https://battleshipapp.azurewebsites.net/api/ConnectPlayer?"+tokenusername,
	        type: "post",
	        success: function (response) {
	           $("#message").html("You are connected! Please press the button below as soon as the second player is connected");
	           playertoken = $("#tokenInput").val();
	           username = $("#usernameInput").val();
	           $(".player-name:first").html(username);
	           $("#remove-on-connect").css("display", "none");
	        },
	        error: function(jqXHR, textStatus, errorThrown) {
	        	// wrong token
	        	console.log(jqXHR);
	        	console.log(textStatus);
	        	console.log(errorThrown);
	           $("#message").html("Wrong token! Please try again!");
	        }
	    });
        ev.preventDefault();
	});
	shipPlace();
	missileLaun();
}

function shipPlace() {
	$("#battleship-body td").each(function() {
		$(this).click(placeShip);
	});
}

function missileLaun() {
	$("#enemy-battleship-body td").each(function() {
		$(this).click(launchMissile);
	});
}

function createGrid(id) {
	var space = 0;
	for (var r=0; r<10; r++) {
	  var tr = document.createElement("tr");
	  for (var c=0; c<10; c++) { 
		var td = document.createElement("td");
		$(td).addClass("cell"+space);
		td.dataPos = space;
		tr.appendChild(td);
		space++;
	  }
	  var grid = document.getElementById(id);
	  grid.appendChild(tr);
	}
}

function removeSelection(x, y) {
	var coordinate = (y*10+x);
	$("#battleship .cell"+coordinate).removeClass("selected");
}

function updateNext(result) {
	console.log(result);
	var done = false;
	if (result['game']['p1']['token']==playertoken) {
		if (result['game']['p1']['doneplacement']==true) {
			done = true;
		}
		else {
			shipTBP = result['game']['p1']['currentShipTBP'];
			indexOfshipTBP = result['game']['p1']['currentShipTBPindex'];
		}
	}
	else {
		if (result['game']['p2']['doneplacement']==true) {
			done = true;
		}
		else {
			shipTBP = result['game']['p2']['currentShipTBP'];
			indexOfshipTBP = result['game']['p2']['currentShipTBPindex'];
		}
	}
	if (done) {
		for (var i = 0; i<100; i++) {
			$("#battleship .cell"+i).prop("onclick", null).off("click");
		}
		$("#message-continued").html("You have finished placing all of your ships, press the button below as soon as the second player is too!");
	}
	else {
		$("#message-continued").html("You have to place ship of type "+shipTBP+" and dimension "+(5-indexOfshipTBP));
	}
}

function placeShip() {
	if (shipPlacement) {
		if (!$(this).hasClass("selected")) {
			$(this).addClass("selected");
			if (!firstClick) {
				firstClick = true;
				firstRow = Math.floor((this.dataPos)/10); 
				firstColumn = (this.dataPos)%10;
			}
			else {
				firstClick = false;
				var endingY = Math.floor((this.dataPos)/10);
				var endingX = (this.dataPos)%10;
				var startingX = firstColumn;
				var startingY = firstRow;
				console.log(startingX);
				console.log(startingY);
				console.log(endingX);
				console.log(endingY);
				var values = "startingX="+startingX+"&startingY="+startingY+"&endingX="+endingX+"&endingY="+endingY+"&token="+playertoken+"&type="+shipTBP;
				$.ajax({
			        url: "https://battleshipapp.azurewebsites.net/api/SetShip?"+values,
			        type: "post",
			        success: function (response) {
			           	//alert("Placed!");
			           	$("#message-error").css("display", "none");
			            var result = JSON.parse(response);
			            console.log(result);
						console.log("Already placed are");
						console.log(placed);
			           	for (var i = Math.min(startingX, endingX); i<= Math.max(startingX, endingX); i++) {
							for (var j = Math.min(startingY, endingY); j<= Math.max(startingY, endingY); j++) {
								var coordinate = (j*10+i);
								console.log(coordinate);
								$("#battleship .cell"+coordinate).prop("onclick", null).off("click");
								$("#battleship .cell"+coordinate).addClass("selected");
								placed.push(coordinate);
							}
						}
						updateNext(result);
			        },
			        error: function(jqXHR, textStatus, errorThrown) {
			           // either a- overlapping ship or b- some other random error (usually cannot be triggered by user input but rather uniquely by API calls)
			           $("#message-error").css("display", "block");
			           removeSelection(startingX, startingY);
			           removeSelection(endingX, endingY);
			        }
			    });
			}
		}
		else {
			console.log("The dataPos of this cell is "+this.dataPos);
			if (!placed.includes(this.dataPos)) {
				$(this).removeClass("selected");
				firstClick = false;
			}
		}
	}
}

function launchMissile() {
	var currentcell = $(this);
	if (missileLaunch) {
		// First, we need to check if it is our turn
		var result = getStateAndReturn();
		console.log("The token of current player is "+playertoken);
		console.log(result);
		if (result['p1']['isTurn']==true&&result['p1']['token']==playertoken
			||result['p2']['isTurn']==true&&result['p2']['token']==playertoken) {
			$("#message-error").css("display", "none");
			var row = Math.floor((this.dataPos)/10);
			var column = this.dataPos%10;
			var values = "hitX="+column+"&hitY="+row+"&token="+playertoken;
			$.ajax({
				url: "https://battleshipapp.azurewebsites.net/api/PlayTurn?"+values,
				type: "post",
				success: function(response) {
					var state = JSON.parse(response);
					// Paint red in any case
					currentcell.addClass("hit");
					// Remove onclick in any case
					currentcell.prop("onclick", null).off("click");
					// Check if game over
					if (state['message'].includes("won")) {
						// Since you played the last move, you won!
						$("#message-continued").html("Congratulations! You have won the game!");
						currentcell.html("X");
						// Disable everything
						$("*").prop("onclick", null).off("click");
						$("#message-error").css("display", "none");
					}
					// We need to check whether you hit or miss
					// This is a hit
					else {
						var enemyLivesLeft;
						if (state['game']['p1']['token']!=playertoken) {
							enemyLivesLeft = state['game']['p1']['livesLeft'];
						}
						else {
							enemyLivesLeft = state['game']['p2']['livesLeft'];	
						}
						if (state['message'].includes("Hit!")) {
							currentcell.html("X");
							$("#message-continued").html("You have hit a ship! Your opponent has "+enemyLivesLeft+" lives left.");
						}
						else {
							$("#message-continued").html("Bad luck! You missed. Your opponent has "+enemyLivesLeft+" lives left.");
						}
					}
				}
			});
		}
		else {
			//console.log("Not your turn!");
			$("#message-error").css("display", "block");
		}
	}
}

function getStateAndReturn() {
	var result;
	$.ajax({
		url: "https://battleshipapp.azurewebsites.net/api/GetState",
		type: "get",
		success: function (response) {
			result = JSON.parse(response);
			//console.log(result)
		},
		error: function(jqXHR, textStatus, errorThrown) {
			alert("Something wrong happened, oops!");		
		},
		async: false
	});
	return result;
}
// This function will refresh the state of the game depending on the interactions done by the opponent
function getState() {
	if (!shipPlacement) {
		$.ajax({
		        url: "https://battleshipapp.azurewebsites.net/api/GetState",
		        type: "get",
		        success: function (response) {
		        	// Check if both players are connected
		            var result = JSON.parse(response);
		            // console.log(result);
		           	// console.log(result['p1']['isConnected']);
		           	// console.log(result['p2']['isConnected']);
		           	// console.log(result['p2']['isConnected']=='true');
		           	// console.log(result['p2']['isConnected']==='true');
		           	// console.log(result['p2']['isConnected']==true);
		           	// console.log(result['p2']['isConnected']===true);
		            if ((result['p1']['isConnected']===true) && (result['p2']['isConnected']===true))
		            {
		            	$("#message").html("Both players are connected! You may now place your ships on the board");
		            	var result = getStateAndReturn();
		            	//console.log(result);
		            	if (result['p1']['token']==playertoken) {
		            		shipTBP = result['p1']['currentShipTBP'];
		            		indexOfshipTBP = result['p1']['currentShipTBPindex'];
		            	}
		            	else {
		            		shipTBP = result['p2']['currentShipTBP'];
		            		indexOfshipTBP = result['p2']['currentShipTBPindex'];
		            	}
		            	$("#message-continued").html("You have to place ship of type "+shipTBP+" and dimension "+(5-indexOfshipTBP));
		            	shipPlacement = true;
		            }
		            else {
		           		$("#message").html("One or more players are not connected yet!");
		            }
		        },
		        error: function(jqXHR, textStatus, errorThrown) {
		           // console.log(jqXHR);
		           // console.log(textStatus);
		           // console.log(errorThrown);
		           $("#message-server").css("display", "block");
		        }
		    });
	}
	else if (!missileLaunch) {
		$.ajax({
		        url: "https://battleshipapp.azurewebsites.net/api/GetState",
		        type: "get",
		        success: function (response) {
		        	// Check if both players are connected
		            var result = JSON.parse(response);
		            if (result['p1']['doneplacement']==true && result['p2']['doneplacement']==true) 
		            {
		            	$("#message").html("Both players have placed the entirety of their ships, you may now proceed to launch your missiles, commander!");
		            	missileLaunch = true;
						$("#message-error").html("It is not your turn now!");
		            	if (result['p1']['token']==playertoken) {
			            	$("#message-continued").html("It is your turn to launch a missile now!");
			            	isTurn = true;
			            	// This is hidden so don't worry if it doesn't make too much sense the way it is written now
		            	}
		            	else {
		            		var opponentname = result['p1']['name'];
		            		$("#message-continued").html("It is now "+opponentname+"'s turn! Please wait until they finish their move...");
		            	}
		            }
		            else {		            	
		           		$("#message").html("One or more players haven't placed all of their ships yet!");
		            }
		        },
		        error: function(jqXHR, textStatus, errorThrown) {
		            $("#message-server").css("display", "block");
		        }
		    });
	}
	else {
		// Here we update where we're hit and whether the game is over or not
		$.ajax({
			url: "https://battleshipapp.azurewebsites.net/api/GetState",
			type: "get",
			success: function (response) {
				var result = JSON.parse(response);
				var playerboardhit;
				var playerboardplaced;
				var livesLeft;
				if (result['p1']['token']==playertoken) {
					// Get the hit board and add Xs where you should
					playerboardhit = result['p1']['board']['hit'];
					playerboardplaced = result['p1']['board']['placed']
					livesLeft = result['p1']['livesLeft'];
				}
				else {
					playerboardhit = result['p2']['board']['hit'];
					playerboardplaced = result['p2']['board']['placed'];
					livesLeft = result['p2']['livesLeft'];
				}
				for (var i = 0; i<10; i++) {
					for (var j = 0; j<10; j++) {
						if (playerboardhit[i][j]==true) {
							var coordinate = (j*10+i);
							if (playerboardplaced[i][j]==true)
								$("#battleship .cell"+coordinate).html("X");
							else 
								$("#battleship .cell"+coordinate).html("-");
						}
					}
				}
				if (livesLeft==0) {
						$("#message-continued").html("Bad news... You lost the battleship war!");
						$("#message-error").css("display", "none");
						// Disable everything
						$("*").prop("onclick", null).off("click");
				}
			}, 
			error: function(jqXHR, textStatus, errorThrown) {
		        $("#message-server").css("display", "block");
		    }
		});
	}
}