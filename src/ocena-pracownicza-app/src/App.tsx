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
import Register from "./pages/Register";
import Stage2ReviewQueue from "./pages/Stage2ReviewQueue";
import Stage2ReviewDetails from "./pages/Stage2ReviewDetails";
import EmployeeEvaluationHistory from "./pages/EmployeeEvaluationHistory";
import EmployeeEvaluationHistoryDetails from "./pages/EmployeeEvaluationHistoryDetails";
import AddAchievementForm from "./pages/AddAchievementForm";
import AchievementList from "./pages/AchievementList";

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
                            <Route path="/register" element={<Register />} />

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
                            <Route
                                path="/evaluation/stage2"
                                element={<Stage2ReviewQueue />}
                            />
                            <Route
                                path="/evaluation/stage2/:achievementId"
                                element={<Stage2ReviewDetails />}
                            />
                            <Route
                                path="/evaluation/history"
                                element={<EmployeeEvaluationHistory />}
                            />
                            <Route
                                path="/evaluation/history/:achievementId"
                                element={<EmployeeEvaluationHistoryDetails />}
                            />
                            <Route
                                path="/achievement/add"
                                element={<AddAchievementForm />}
                            />
                            <Route
                                path="/achievements"
                                element={<AchievementList />}
                            />
                        </Route>
                    </Routes>
                </BrowserRouter>
            </AuthProvider>
        </>
    );
}

export default App;
