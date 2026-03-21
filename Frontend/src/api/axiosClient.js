import axios from 'axios';

const axiosClient = axios.create({
    baseURL: 'http://localhost:5136/api', // Dùng HTTP để tránh lỗi SSL Chứng chỉ đỏ trên local
    headers: {
        'Content-Type': 'application/json'
    },
    withCredentials: true // Rất quan trọng để đính kèm HttpOnly cookies (RefreshToken)
});

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
    failedQueue.forEach(prom => {
        if (error) {
            prom.reject(error);
        } else {
            prom.resolve(token);
        }
    });

    failedQueue = [];
};

axiosClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('accessToken');
        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

axiosClient.interceptors.response.use(
    (response) => {
        return response;
    },
    async (error) => {
        const originalRequest = error.config;

        if (error.response?.status === 401 && !originalRequest._retry) {
            if (isRefreshing) {
                return new Promise(function(resolve, reject) {
                    failedQueue.push({ resolve, reject });
                }).then(token => {
                    originalRequest.headers['Authorization'] = 'Bearer ' + token;
                    return axiosClient(originalRequest);
                }).catch(err => {
                    return Promise.reject(err);
                });
            }

            originalRequest._retry = true;
            isRefreshing = true;

            try {
                // Call refresh-token API. Cookie is automatically sent due to withCredentials
                const { data } = await axios.post('http://localhost:5136/api/auth/refresh-token', {}, { withCredentials: true });
                
                const newAccessToken = data.data.accessToken;
                localStorage.setItem('accessToken', newAccessToken);
                
                axiosClient.defaults.headers.common['Authorization'] = `Bearer ${newAccessToken}`;
                originalRequest.headers['Authorization'] = `Bearer ${newAccessToken}`;
                
                processQueue(null, newAccessToken);
                return axiosClient(originalRequest);
            } catch (err) {
                processQueue(err, null);
                // Refresh token failed (expired or invalid), force logout
                localStorage.removeItem('accessToken');
                localStorage.removeItem('user');
                window.location.href = '/login'; // Redirect to login
                return Promise.reject(err);
            } finally {
                isRefreshing = false;
            }
        }

        return Promise.reject(error);
    }
);

export default axiosClient;
