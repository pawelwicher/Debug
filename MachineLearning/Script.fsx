let D = 100.0
let d = 80.0
let r = 10.0
let h = (D - d) / 2.0
let h_r = h / r

let c1 = 1.007 + 1.0 * sqrt h_r - 0.031 * h_r
let c2 = -0.27 - 2.404 * sqrt h_r + 0.749 * h_r
let c3 = 0.667 + 1.133 * sqrt h_r - 0.904 * h_r
let c4 = -0.414 + 0.271 * sqrt h_r - 0.186 * h_r

let K = c1 + c2 * (2.0 * h / D) + c3 * (2.0 * h / D) ** 2.0 + c4 * (2.0 * h / D) ** 3.0