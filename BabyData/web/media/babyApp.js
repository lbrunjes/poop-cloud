	angular.module('babyApp', [])
		.controller('babyController', function($scope, $http) {
			var babyController = this;
			this.baby={};
			this.user={};
			this.glance = {};
			this.search = {};
			this.chartform = {};
			this.customEvent ={};
			this.eventTypeBlacklist =["event"];
			
			this.ServiceUrl="../Service.ashx";


			//called at init
			this.init =function(bd){
				
			//	console.log(bD.query);
				this.search = this.readQueryString();

				this.getBaby(this.search.baby);
				this.getUserData();
					
				window.bD = new babyData("theChart");
				bD.updateChart([] ,{
						height:document.getElementById("theChart").height,
						width:document.getElementById("theChart").width,
						graph_type:"bar",
						background:false,
						label_axis_x:"Number",
						label_axis_y:"Days Ago"
					});
			};

			//gets data for the baby.
			this.getEventData = function(babyId){
				$http.get(this.ServiceUrl+"?type=babyevents&id="+babyId).then(

					function(response){
						var events = response.data.events;
				
						for(var i =0; i < events.length; i++){
							babyController.addEvent(events[i]);
						}
					},
					function(response){
						console.log("error @ load events", response);

					}
					
				);
					
				//calculate time_agoago
			};

			//adds data to the UI
			this.addEvent = function(event){

				if(!babyController.baby.events){
					babyController.baby.events = [];
				}
				if(babyController.baby.events.indexOf(event)<0){
					babyController.baby.events.push(event);
					if(!babyController.glance[event.type] ||
						babyController.glance[event.type].time < event.time){
						babyController.glance[event.type] = event;
					}
					
				}

			};
			this.addButton =function(type, subtype, details){
				var butt ={"type":type, "subtype":subtype,"details":details};
				babyController.buttons.push(butt);

			}

			//adds data to the db
			this.reportEvent=function(type, subtype, details){

				$http.post(this.ServiceUrl+
					"?type=babyevents"+
					"&id="+babyController.user.id+
					"&eventtype="+ type +
					"&subtype="+ subtype||"" +
					"&details="+ details||"").then(
						function(response){
							console.log("reported event");
							babyController.addEvent(response.data.events[0]);
						},
						function(response){
							console.log("error @ report event", response);
							babyController.addEvent("ERROR",
							response.data.server_error.type+": " +
								response.data.server_error.message);
							alert(response.data.server_error.message);
						}
					);
			};

		



			//used to do tabbed interface.
			this.showTab=function(ele_id,e){
				var tgt = document.getElementById(ele_id);
				if(tgt){
					var tabs = document.getElementsByClassName("tab");
					for(var i = 0; i < tabs.length;i++){
						tabs[i].classList.remove("show");
						tabs[i].classList.add("hide");
					}
					tgt.classList.add("show");
					tgt.classList.remove("hide");
					
					tabs = document.getElementsByClassName("tab-button");
					for(var i = 0; i < tabs.length;i++){
						tabs[i].classList.remove("active");
					}
					e.target.classList.add("active");
				}
			};
			//Gets user data
			this.getUserData=function(username){
				var un = username||"";
				$http.get(this.ServiceUrl+"?type=user&id="+un).then(
					function(response){
						babyController.user = response.data;
						console.log("x");
						if(!babyController.user.displaydata){
							babyController.user.displaydata={
								"button":[
									{"type":"feeding", "subtype":"breast","details":""},
									{"type":"feeding", "subtype":"bottle","details":""},
									{"type":"sleep", "subtype":"up","details":"awoke, likely screaming"},
									{"type":"sleep", "subtype":"down","details":"asleep, mercifully"},
									{"type":"diaper", "subtype":"poo","details":""},
									{"type":"diaper", "subtype":"pee","details":""}
								],
								"color":{
									"feeding":"#B2AC75",
									"diaper":"#FFCCE2",
									"sleep":"#6AE7FF",
									"info":"#00000f"
								}
							}
						}

						for(var key in babyController.glance){
							var colors= babyController.user.displaydata.color;

							if(!colors[key.toLowerCase()]){
								colors[key.toLowerCase()] = bD.getColorFromString(key);

							}
							
						}
						//pass by ref means updaign one updates both.
						babyController.colors = babyController.user.displaydata.color;
						babyController.buttons = babyController.user.displaydata.button;
					},
					function(response){
						console.log("error @ load user", response);
						babyController.addEvent("ERROR",
							response.data.server_error.type+": " +
								response.data.server_error.message);

					}
				);

			};
			//adds data to the db
			this.saveUser=function(){

				$http.post(this.ServiceUrl+
					"?type=user"+
					"&id="+babyController.user.username+
					"&email="+ babyController.user.email +
					"&image="+ babyController.user.image +
					"&displaydata="+ JSON.stringify(babyController.user.displaydata) //this line is bad for some reason.
					).then(
						function(response){
							console.log("reported event");
							
						},
						function(response){
							console.log("error @ report event", response);
							babyController.addEvent("ERROR",
							response.data.server_error.type+": " +
								response.data.server_error.message);
							alert(response.data.server_error.message);
						}
					);
			};
			//get requested baby
			this.getBaby=function(babyId){
				$http.get(this.ServiceUrl+"?type=baby&id="+babyId).then(
					function (response){
						babyController.baby= response.data;
						babyController.getEventData(babyController.baby.id);

					},
					function (response){
						console.log("error @ baby load", response);
						babyController.baby= {
							"name":"ERROR LOADING BABY",
							"events":[]
						};
						//show on the baby list.
						babyController.addEvent("ERROR",
							response.data.server_error.type+": " +
								response.data.server_error.message);
						document.getElementById("baby-header").classList.add("error");
						
					}
				);
			};

			//update teh chart
			this.updateChart=function(){
				if(!this.chartform){
					console.log("no data to chart");
					return;
				}

				console.log("updaing charts with data", this.chartform);
				//TODO get data from the baby
				
				var now = new Date();
				var ms_day = 1000*3600*24;
				var days = this.chartform.days||1;
				var ms = ms_day +days;
				var data = {};
				var labels =[];

				for(var  i = days-1; i >= 0;i--){
					labels.push(i)//TODOd dates
				}

				
				for(var key in this.glance){
					if(this.chartform[key]){
						var info = [];
						for(var i = 0; i < days; i++){
							info.push(0);
						}
						data[key] = info;
					}
				}
				console.log(data);

				var evt,ago,tgt;
				for(var i = 0; i < this.baby.events.length; i++){
					evt= this.baby.events[i];
					if(evt.type in data){
						ago = Math.abs(Date.parse(evt.time) - now);
						//are we in teh windw
						
						if(ago){
							tgt = Math.floor(days-(ago+1)/ms_day);
							data[evt.type][tgt]++;
						}
					}
				}

				var dataset =[];
				for(var key in data){
					dataset.push({
						"color": bD.getColorFromString(key),
						"name":key,
						"items":data[key],
						"labels":labels});
				}	
				console.log("datset",dataset);

			

				
				bD.updateChart(dataset,
					{
						height:document.getElementById("chart").height,
						width:document.getElementById("chart").width,
						graph_type:days<7?"bar":"line",
						background:false,
						label_axis_x:"Number",
						label_axis_y:"Days Ago",
						title:"Events by type for Last "+days + " days"
					}
				);
			};

			//turn time stamp into a time ago by major type
			this.getAgo=function(timestamp){
				var ts = Math.abs(Date.parse(timestamp) - new Date());
				
				if(ts){
					if(ts <1000){
						ts ="now"
					}
					else if(ts < 60000){ //1 minute
						ts = Math.round(ts/1000) +" seconds ago";

					}
					else if(ts < 3600000){ //1 hour
						ts = Math.round(ts/60000) +" minutes ago";

					}
					else if(ts < 86400000){ //1 day
						ts = Math.round(ts/3600000) +" hours ago";

					}
					else{
						ts = Math.round(ts/86400000) +" days ago";
					}
				}
				return ts;
			};

			//twhat it says
			this.readQueryString = function(){
				var search = {};
				var query = unescape(window.location.search.substring(1)).split("&");
				for(var i = 0 ; i < query.length;i++){

					var eq= query[i].indexOf('=');
					search[query[i].substring(0,eq)] = query[i].substring(eq+1);
				}
				return search;
			}


			this.init();
		});