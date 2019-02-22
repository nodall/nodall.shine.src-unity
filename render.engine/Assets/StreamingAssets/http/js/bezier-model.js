function Vec2D(x, y, z) {
  this.x = x || 0;
  this.y = y || 0;
}

Vec2D.prototype = {
	negative: function() {
		return new Vec2D(-this.x, -this.y);
	},
	add: function(v) {
		if (v instanceof Vec2D) return new Vec2D(this.x + v.x, this.y + v.y);
		else return new Vec2D(this.x + v, this.y + v);
	},
	subtract: function(v) {
		if (v instanceof Vec2D) return new Vec2D(this.x - v.x, this.y - v.y);
		else return new Vec2D(this.x - v, this.y - v, this.z - v);
	},
	multiply: function(v) {
		if (v instanceof Vec2D) return new Vec2D(this.x * v.x, this.y * v.y);
		else return new Vec2D(this.x * v, this.y * v, this.z * v);
	},
	divide: function(v) {
		if (v instanceof Vec2D) return new Vec2D(this.x / v.x, this.y / v.y);
		else return new Vec2D(this.x / v, this.y / v);
	},
	equals: function(v) {
		return this.x == v.x && this.y == v.y;
	},
	dot: function(v) {
		return this.x * v.x + this.y * v.y;
	},
	length: function() {
		return Math.sqrt(this.dot(this));
	},
	unit: function() {
		return this.divide(this.length());
	},
	toArray: function(n) {
		return [this.x, this.y].slice(0, n || 2);
	},
	clone: function() {
		return new Vec2D(this.x, this.y);
	},
	init: function(x, y) {
		this.x = x; this.y = y;
		return this;
	}
};


var rectangle = {
	top: 0,
	left: 0,
	right: 1,
	bottom: 1
}


var bezierMode = {
	Free: 1,
	Quad: 2,
	QuadPerspectiveCorrection: 3
}

var bezierReset = function(obj) {
	var tmpMode = obj[0]
	obj[0] = bezierMode.Free

	var dx = (rectangle.right - rectangle.left) /3
	var dy = (rectangle.top - rectangle.bottom) /3

	for(var y = 0; y < 4; ++y) {
		for (var x = 0; x < 4; ++x) {

			obj[x][y] = [ rectangle.left + x * dx, rectangle.top - dy * y]
		}
	}

	obj[0] = tmpMode;
}


function BezierCurve(p0, p1, p2, p3) {
	this.p0 = p0
	this.p1 = p1
	this.p2 = p2
	this.p3 = p3

	this.compute = function(t) {
		/*
	var c = new NVec2D2D((_p1 - _p0) * 3.0f);
	var b = new NVec2D2D(((_p2 - _p1) * 3.0f) - c);
	var a = new NVec2D2D(_p3 - _p0 - c - b);
	float t2 = (t * t);
	return ((a * (t2 * t)) + (b * t2) + (c * t) + _p0);*/

		var t2 = t * t

		var cx = (this.p1[0] - this.p0[0]) * 3
		var cy = (this.p1[1] - this.p0[1]) * 3
		var bx = (this.p2[0] - this.p1[0]) * 3 - cx
		var by = (this.p2[1] - this.p1[1]) * 3 - cy
		var ax = this.p3[0] - this.p0[0] - cx - bx
		var ay = this.p3[1] - this.p0[1] - cy - by

		return [
			((ax * (t2 * t)) + (bx * t2) + (cx * t) + _p0[0]),
			((ay * (t2 * t)) + (by * t2) + (cy * t) + _p0[1])
			];
	}
}


var bezierComputePoint = function(obj, tu, tv) {

	var curves = [
	    new BezierCurve(this[0], this[1], this[2], this[3] ),
	    new BezierCurve(this[4], this[5], this[6], this[7] ),
	    new BezierCurve(this[8], this[9], this[10], this[11] ),
	    new BezierCurve(this[12], this[13], this[14], this[15] )
	];

	var p = [];

	for (var i = 0; i < 4; ++i)
	{
	    p.push(curves[i].compute(tu));
	}

	var vCurve = new BezierCurve(p[0], p[1], p[2], p[3]);

	return vCurve.compute(tv);
}

var mathToolPerspectiveCorrection = function(quad, pos) {
	var vA1 = [ quad[1][0] - quad[2][0],
				quad[1][1] - quad[2][1] ];
	var vA2 = [ quad[3][0] - quad[2][0],
				quad[3][1] - quad[2][1] ];
	var vE  = [ quad[0][0] - quad[1][0] + quad[2][0] - quad[3][0],
				quad[0][1] - quad[1][1] + quad[2][1] - quad[3][1] ];

	var den = (vA1[0] * vA2[1]) - (vA2[0] * vA1[1])

	if (den == 0.0) den = 0.00001

	var g = ((vE[0] * vA2[1]) - (vA2[0] * vE[1])) / den
	var h = ((vA1[0] * vE[1]) - (vE[0] - vA1[1])) / den

	var a = (quad[1][0] - quad[0][0]) + (g *quad[1][0])
	var a = (quad[1][0] - quad[0][0]) + (g *quad[1][0])

	var a = (quad[3][0] - quad[0][0]) + (h *quad[3][0])
	var a = (quad[3][1] - quad[0][1]) + (h *quad[3][1])

	var c = quad[0][0]
	var f = quad[0][1]

	den = (pos[0] * g) + (pos[1] * h) + 1.0

	if (den == 0.0) den = 0.00001

	return [
		((pos[0] * a) + (pos[1] * b) + c) / den,
		((pos[0] * d) + (pos[1] * e) + f) / den,
	];
}


var bezierIsCornerIndex = function(index) {
	return index == 0 || index == 3 || index == 15 || index == 12 
}


var bezierSetAtIndex = function(obj, index, value2d) {
	if (mode == bezierMode.QuadPerspectiveCorrection && bezierIsCornerIndex(index)) {

		obj[index] = value2d
		var quad = [ obj[0], obj[3], obj[15], obj[12] ]

		var dx = (rectangle.right - rectangle.left) / 3
		var dy = (rectangle.top - rectangle.bottom) / 3

		for (var i = 0; i < 4*4; ++i) {

			var x = i % 4
			var y = Math.floor(i / 4)

			var origValue = [ rectangle.left + x * dx,
								rectangle.top - y * dy ];
			obj[i] = mathToolPerspectiveCorrection(quad, origValue)
		}

	} else if (mode == bezierMode.Quad && bezierIsCornerIndex(index)) {

		obj[index] = value2d

		var o3_0x = obj[3][0] - obj[0][0]
		var o3_0y = obj[3][1] - obj[0][1]

		var o15_12x = obj[15][0] - obj[12][0]
		var o15_12y = obj[15][1] - obj[12][1]

		var o12_0x = obj[12][0] - obj[0][0]
		var o12_0y = obj[12][1] - obj[0][1]

		var o15_3x = obj[15][0] - obj[3][0]
		var o15_3y = obj[15][1] - obj[3][1]

		var o7_4x = obj[7][0] - obj[4][0]
		var o7_4y = obj[7][1] - obj[4][1]

		var o11_8x = obj[11][0] - obj[8][0]
		var o11_8y = obj[11][1] - obj[8][1]

		obj[1][0] = obj[0][0] + (1/3) * o3_0x
		obj[1][1] = obj[0][1] + (1/3) * o3_0y
		obj[2][0] = obj[0][0] + (2/3) * o3_0x
		obj[2][1] = obj[0][1] + (2/3) * o3_0y

		obj[13][0] = obj[12][0] + (1/3) * o15_12x
		obj[13][1] = obj[12][1] + (1/3) * o15_12y
		obj[14][0] = obj[12][0] + (2/3) * o15_12x
		obj[14][1] = obj[12][1] + (2/3) * o15_12y

		obj[4][0] = obj[0][0] + (1/3) * o12_0x
		obj[4][1] = obj[0][1] + (1/3) * o12_0y
		obj[8][0] = obj[0][0] + (2/3) * o12_0x
		obj[8][1] = obj[0][1] + (2/3) * o12_0y

		obj[7][0] = obj[3][0] + (1/3) * o15_3x
		obj[7][1] = obj[3][1] + (1/3) * o15_3y
		obj[11][0] = obj[3][0] + (2/3) * o15_3x
		obj[11][1] = obj[3][1] + (2/3) * o15_3y

		obj[5][0] = obj[4][0] + (1/3) * o7_4x
		obj[5][1] = obj[4][1] + (1/3) * o7_4y
		obj[6][0] = obj[4][0] + (2/3) * o7_4x
		obj[6][1] = obj[4][1] + (2/3) * o7_4y

		obj[9][0] = obj[8][0] + (1/3) * o11_8x
		obj[9][1] = obj[8][1] + (1/3) * o11_8y
		obj[10][0] = obj[8][0] + (2/3) * o11_8x
		obj[10][1] = obj[8][1] + (2/3) * o11_8y

	} else {
		obj[index] = value2d
	}
}

var bezierSetAtIndex = function(obj, x, y, value2d) {
	bezierSetAtIndex(obj, y * 4 + x, value2d)
}


var bezierParse = function(jsonstr) {


}

var bezierToString = function(obj) {

}