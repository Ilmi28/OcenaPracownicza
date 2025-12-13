import { Typography, Paper, Box } from "@mui/material";
import { authService } from "../services/authService";
import axiosClient from "../services/axiosClient";

const Dashboard = () => {
    const testLogin = async () => {
        try {
            await authService.login({
                userNameEmail: "admin",
                password: "admin",
            });
            console.log("LOGIN OK");
        } catch (e) {
            console.log("LOGIN ERROR", e);
        }
    };

    const testSecure = async () => {
        try {
            const res = await axiosClient.get("/auth/secure");
            console.log("SECURE OK:", res.data);
        } catch (e) {
            console.log("SECURE ERROR:", e);
        }
    };

    return (
        <Box>
            <Typography variant="h4" gutterBottom>
                Dashboard
            </Typography>
            <Paper sx={{ p: 3 }}>
                <Typography component="div">
                    Lorem ipsum dolor sit amet consectetur adipisicing elit.
                    Quis, facere adipisci, voluptate esse numquam nobis eligendi
                    asperiores voluptatibus praesentium itaque cum omnis,
                    repellat officia ipsam aperiam recusandae! Ipsa, minima
                    eveniet.
                    <div style={{ padding: 20 }}>
                        <button onClick={testLogin}>Test Login</button>
                        <button onClick={testSecure}>Test Secure</button>
                    </div>
                </Typography>
            </Paper>
        </Box>
    );
};
export default Dashboard;
