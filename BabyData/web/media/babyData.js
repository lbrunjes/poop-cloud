var babyData = function(id){

	this.search = "";
	this.context = null;
	this.baby = "";

	//setup
	this.init = function(id){
		var ele = document.getElementById(id);
		ele.width = document.getElementById(id).width||640;
		ele.height  = document.height  -256|| 480 ;

		this.context = ele.getContext("2d");

		this.search = this.readQueryString();
		this.baby = this.search.baby|"UNKNOWN";

	};

	//gets a color for css use from a string
	this.getColorFromString = function(input,type){
		var colors =[0,0,0];
		if(type == "rgba"){
			colors.push(0);
		}
		for(var i = 0; i < input.length;i++){
			var idx = i % colors.length;
			var bits = input.charCodeAt(i);
			
				colors[idx] += Math.min(255,colors[idx] + bits);

		}
		var d = input.length /colors.length;

		for(var i = 0; i <colors.length;i++){
			colors[i] = Math.round(colors[i]/d);
		};

		var outval = "";
		switch(type){
			case "rgba":
			var a= colors.pop();
			outval= "rgba(" + colors.join(",") +","+ 
				Math.max(0.3,Math.round(colors[3]/255))+")";
			break;
		case "rgb":
			outval= "rgb(" + colors.join(",") +")";
			break;
		case "hsl":
			//this is less than perfect...
			outval = "hsl(" + Math.round(colors[0]/255*360) + ","+
				 Math.round(colors[1]/255 *100)+"%,"+
				 Math.round(colors[2]/255 *100)+"%)";
			break;
		case "hex":
		default:
			outval=  "#"+ colors[0].toString(16)
				+ colors[1].toString(16)
				+ colors[2].toString(16);
			break;
		}
			return outval;
	};

	//a filter to make sure datasets have all teh stuff they need
	this.dataset =function(name, items ,color){
		var ids  =[];
		var _color= color;
		if(!_color){
			_color=this.getColorFromString(name);
		}
		for(var idx in items){
			ids.push(idx);
		}
		return{
			"color":_color, 
			"items":items,
			"labels":ids, 
			"name":name};
	};
	this.readQueryString = function(){
		var search = {};
		var query = unescape(window.location.search.substring(1)).split("&");
		for(var i = 0 ; i < query.length;i++){

			var eq= query[i].indexOf('=');
			search[query[i].substring(0,eq)] = query[i].substring(eq+1);
		}
		return search;
	}



	this.updateChart =function(data, options){
		var context = this.context;
		console.log("ceating chart for", data, options);
		var opts = {
			font:"16px Ubutunu, Arial, sans-serif",
			font_size:14,

			labels:true,
			label_color:"#fff",
			label_axis_x:"x axis",
			label_axis_y:"y axis",
			title:"title",
		

			lines :true,
			line_color:"rgba(255,255,255,0.5)",
			line_labels:true,
			line_number:10,
			line_round_at:10,
			range_multiplier: 1.5,
			line_at_zero:true, //TODO
			line_weight:3,

			key:true,

			background:true,
			background_color:"#000",

			graph_type:"bar",
			graph_highlight:true,
			graph_highlight_r:3,
			graph_highlight_color:"rgba(255,255,255,0.5)",

			width:this.context.width,
			height:this.context.height,


			

		};

		//update our options
		if(options){
			for(item in opts){
				if(options[item]!==undefined){
					opts[item] =options[item];
				//	console.log("updated ",item);
				}

			}
		}
	//	console.log("opts",opts)
		
		//setup the context.
		context.save();

		context.font =opts.font;
		context.width = opts.width;
		context.height = opts.height;

		//investigate the data.
		var range = 0;
		var range_graph = 0;;
		var min =  999999;
		var max = -999999;
		var points = 0;
		var point_src = 0;
		for(var i  =0; i < data.length; i++){
			if(data[i].items.length> points){
				points = data[i].items.length;
				point_src = i;
			}
			for(var j  =0; j < data[i].items.length; j++){
				max = Math.max(data[i].items[j], max);
				min = Math.min(data[i].items[j], min);
			}
		}

		range = Math.max(max - min, 0.1) ;
		range_graph = range *opts.range_multiplier;

	//	console.log("range", range, "min", min, "max", max, "points", points);
		
		//is there any data
		if(points==0){
	//		console.log("no data => Exiting")
			context.fillStyle = opts.label_color;
			context.fillText("NO DATA - USE FORM TO ADD DATA",context.width/2, context.height/2);
			context.restore();
			return;
		}


		

		//background
		if(opts.background){
			context.fillStyle = opts.background_color;
			context.fillRect(0,0, opts.width, opts.height);
		}
		else{
			context.clearRect(0,0, opts.width, opts.height);	
		}

		//calculate the grid.
		var left_pad = Math.max(max.toString().length +2)*opts.font_size;
		var bottom_pad = opts.font_size * 5;
		var interior_h = opts.height - bottom_pad;
		var interior_w = opts.width - left_pad;

		var y_step_inc = Math.round(
			(range_graph / opts.line_number));
		var y_step = Math.round(interior_h/(range_graph/y_step_inc));
		var y_start = Math.round(range/2 + opts.line_number/-2 * y_step_inc);

		var x_step = Math.floor(interior_w/(points -1));

		if( opts.graph_type == "bar"){
			x_step = Math.floor((opts.width - left_pad)/(points));
		}
	
		
	//	console.log("left_pad", left_pad, "bottom_pad", bottom_pad, "x_step",x_step, "y_step", y_step,"y_step_inc", y_step_inc, "y_start",y_start);
		

		context.save();
		context.translate(left_pad,0);

		//add lines
		if(opts.lines){
			
			//horizontal
			context.textAlign = "right";

			context.save();
			context.translate(0, interior_h);

			for(var y = 0; y < Math.ceil(opts.line_number * opts.range_multiplier); y++){
				context.fillStyle = opts.line_color;

				
				context.fillRect(0, 0,interior_w, 1);
				
			
				if(opts.line_labels){
					context.fillStyle = opts.label_color;
					context.fillText(y *y_step_inc+ y_start ,
						opts.font_size/-2, 0 );
				}

				context.translate(0, - y_step );

			}
			context.restore();

			//vertical
			context.textAlign = "center";

			for(var x = 0; x < points; x++){
				context.fillStyle = opts.line_color;

				context.fillRect(x_step *x, 0 ,
					1,opts.height - bottom_pad);
			
				if(opts.line_labels){
					context.fillStyle = opts.label_color;
				
					if(opts.graph_type =="bar"){
						context.fillText(data[point_src].labels[x],
							x_step *(x +0.5)  , 
							opts.height - bottom_pad +opts.font_size *1.5);

					}
					else{
						context.fillText(data[point_src].labels[x],
							x_step *x , 
							opts.height - bottom_pad +opts.font_size *1.5);	
					}
					
			
				}

			}
			
		}

		//draw key

		if(opts.key){
			context.textAlign = "left";
			context.save()
			context.translate(0 , y_step/3*2);
			for(var i = 0 ; i <data.length;i++){
				context.fillStyle = data[i].color;
				context.fillText( " █ "+data[i].name, 0,0);
				var next = Math.ceil(context.measureText(" █ "+data[i].name).width);
				context.translate(next, 0);

			}
			context.restore();

		}

		//draw lines?
		context.save();
		context.translate(0, opts.height - bottom_pad);
		var tgt_scale=  (opts.height - bottom_pad)/range_graph;
		context.scale(1,-1 * tgt_scale);
		context.translate(0, -y_start);			

		for(var d = 0; d < data.length; d++){
			context.fillStyle = data[d].color;
			context.strokeStyle = data[d].color;
			context.beginPath();
			var barWidth = x_step/points;


			//start line at the right place
			context.moveTo(0,data[d].items[0] );

			for(var i = 0; i < data[d].items.length; i++){


				switch(opts.graph_type){

					case "line":
						context.lineTo(i*x_step + d, 
							data[d].items[i]);
					break;
					case "bar":
					default:
						context.fillRect( i*x_step + d* barWidth, y_start, barWidth, data[d].items[i] -y_start );
				
					break;
				}
			}
			context.lineWidth = opts.line_weight/tgt_scale;
			context.stroke();
		}
		context.restore();


		//hide padding
		context.restore();

		//add axis lavels.
		if(opts.labels){
			context.textAlign = "center";

			context.fillStyle = opts.label_color;

			context.save();
			
			context.translate(opts.font_size, opts.height /2);
			context.rotate(Math.PI/2);
			context.fillText(opts.label_axis_x,0,0);
			context.restore();

			context.textAlign = "center";
			context.fillText(opts.label_axis_y,
				opts.width/2 + left_pad/2, opts.height- opts.font_size/2);

			context.fillText(opts.title,
				opts.width/2+ left_pad/2,  opts.font_size*1.5);

		}
		context.restore();
	}
	//call setup.
	this.init(id);
};