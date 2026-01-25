import "./App.css";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import Dashboard from "./pages/Dashboard";
import Users from "./pages/Users";
import Settings from "./pages/Settings";
import Login from "./pages/Login";
import ResetPassword from "./pages/ResetPassword";
import AdminProfile from "./pages/AdminProfile";
import EmployeeDashboard from "./pages/EmployeeDashboard";
import ManagerDashboard from "./pages/ManagerDashboard";
import { AuthProvider } from "./hooks/AuthProvider";

function App() {
    return (
        <>
            <AuthProvider>
                <BrowserRouter>
                    <Routes>
                        <Route path="/" element={<MainLayout />}>
                            <Route index element={<Dashboard />} />
                            <Route path="users" element={<Users />} />
                            <Route path="settings" element={<Settings />} />
                            <Route path="/login" element={<Login />} />
                            <Route
                                path="/reset-password"
                                element={<ResetPassword />}
                            />
                            <Route path="/admin" element={<AdminProfile />} />
                            <Route
                                path="/employee"
                                element={<EmployeeDashboard />}
                            />
                            <Route
                                path="/manager"
                                element={<ManagerDashboard />}
                            />
                        </Route>
                    </Routes>
                </BrowserRouter>
            </AuthProvider>
        </>
    );
}

export default App;
