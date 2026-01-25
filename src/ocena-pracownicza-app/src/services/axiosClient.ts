import axios from "axios";

const axiosClient = axios.create({
    baseURL: "https://localhost:5000/api",
    withCredentials: true,
    headers: {
        "Content-Type": "application/json",
    },
});

export default axiosClient;
