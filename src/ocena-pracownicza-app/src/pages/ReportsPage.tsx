import React, { useEffect, useState, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import axiosClient from "../services/axiosClient";

interface EvaluationPeriodBasicInfo {
    id: string;
    name: string;
    isClosed: boolean;
}

interface UserInfo {
    userId: string;
    userName: string;
    email: string;
    role: string;
    roles: string[];
}

interface EmployeeDropdownItem {
    id: string;
    firstName: string;
    lastName: string;
}

const ReportsPage: React.FC = () => {
    const navigate = useNavigate();
    const [periods, setPeriods] = useState<EvaluationPeriodBasicInfo[]>([]);
    const [selectedPeriodId, setSelectedPeriodId] = useState<string>("all");
    const [currentUser, setCurrentUser] = useState<UserInfo | null>(null);
    const [isEmployee, setIsEmployee] = useState(false);
    
    const [employees, setEmployees] = useState<EmployeeDropdownItem[]>([]);
    const [selectedEmployeeId, setSelectedEmployeeId] = useState<string>("");

    const [loading, setLoading] = useState(true);
    const [reporting, setReporting] = useState(false);

    useEffect(() => {
        const fetchInitialData = async () => {
            try {
                setLoading(true);
                const userResp = await axiosClient.get<UserInfo>("/auth/me");
                setCurrentUser(userResp.data);
                const employeeCheck = userResp.data.role === "Employee";
                setIsEmployee(employeeCheck);

                if (employeeCheck) {
                    setSelectedEmployeeId(userResp.data.userId);
                }

                const periodsResp = await axiosClient.get<EvaluationPeriodBasicInfo[]>("/evaluation-periods");
                setPeriods(periodsResp.data);

                if (periodsResp.data.length > 0) {
                    const activePeriod = periodsResp.data.find(p => !p.isClosed);
                    if (activePeriod) {
                        setSelectedPeriodId(activePeriod.id);
                    } else {
                        setSelectedPeriodId(periodsResp.data[0].id);
                    }
                }

                if (!employeeCheck) {
                    const empResp = await axiosClient.get<EmployeeDropdownItem[]>("/achievement/employee-dropdown");
                    setEmployees(empResp.data);
                    if (empResp.data.length > 0) {
                        setSelectedEmployeeId(empResp.data[0].id);                
                    }
                }
            } catch (err) {
                console.error("Błąd podczas ładowania widoku raportów", err);
            } finally {
                setLoading(false);
            }
        };

        fetchInitialData();
    }, []);

    const downloadReport = async (subPath: string, defaultFileName: string) => {
        if (!selectedEmployeeId && subPath.includes("employee/")) {
            alert("Proszę najpierw wybrać pracownika.");
            return;
        }

        setReporting(true);
        try {
            const hasPeriod = selectedPeriodId !== "all";
            const periodParam = hasPeriod ? `evaluationPeriodId=${selectedPeriodId}` : "";
            const separator = subPath.includes("?") ? "&" : "?";
            
            const fullUrl = `/reports/${subPath}${hasPeriod ? separator + periodParam : ""}`;
            
            const response = await axiosClient.get(fullUrl, {
                responseType: "blob"
            });

            const blob = new Blob([response.data], { type: response.headers["content-type"] });
            const link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            link.download = defaultFileName;
            link.click();
            window.URL.revokeObjectURL(link.href);
        } catch (error) {
            console.error("Błąd generowania pliku raportu", error);
            alert("Nie udało się pobrać raportu. Upewnij się, że wybrany pracownik posiada zatwierdzone osiągnięcia w tym okresie.");
        } finally {
            setReporting(false);
        }
    };

    const selectedPeriodNameClean = useMemo(() => {
        const p = periods.find(x => x.id === selectedPeriodId);
        return p ? p.name.replace(/\s+/g, "_") : "Wszystkie_Okresy";
    }, [periods, selectedPeriodId]);

    const selectedEmployeeNameClean = useMemo(() => {
        if (isEmployee) return currentUser?.userName?.replace(/\s+/g, "_") || "Pracownik";
        const emp = employees.find(x => x.id === selectedEmployeeId);
        return emp ? `${emp.lastName}_${emp.firstName}` : "Pracownik";
    }, [employees, selectedEmployeeId, isEmployee, currentUser]);

    if (loading) return <div className="loader-reports">Przygotowywanie centrum raportowego...</div>;

    return (
        <div className="reports-view-container">
            <header className="reports-view-header">
                <div>
                    <button type="button" onClick={() => navigate(-1)} className="reports-back-btn">← Wróć</button>
                    <h1>Raporty</h1>
                    <p className="reports-subtitle">Generuj i pobieraj arkusze okresowej oceny pracowników</p>
                </div>
            </header>

            <PaperCard>
                <div className="filter-card-content">
                    {               }
                    <div className="filter-box-item">
                        <label htmlFor="report-period-select">Okres oceniania:</label>
                        <select 
                            id="report-period-select"
                            value={selectedPeriodId} 
                            onChange={(e) => setSelectedPeriodId(e.target.value)}
                            className="select-reports-dropdown"
                        >
                            <option value="all">Wszystkie zarejestrowane okresy</option>
                            {periods.map(p => (
                                <option key={p.id} value={p.id}>
                                    {p.name} {p.isClosed ? "(Zarchiwizowany)" : "(Aktywny)"}
                                </option>
                            ))}
                        </select>
                    </div>

                    {                                    }
                    {!isEmployee && (
                        <div className="filter-box-item">
                            <label htmlFor="report-employee-select">Wybierz pracownika do raportu indywid.:</label>
                            <select 
                                id="report-employee-select"
                                value={selectedEmployeeId} 
                                onChange={(e) => setSelectedEmployeeId(e.target.value)}
                                className="select-reports-dropdown employee-select"
                            >
                                {employees.length === 0 ? (
                                    <option value="" disabled>Brak pracowników w bazie</option>
                                ) : (
                                    employees.map(e => (
                                        <option key={e.id} value={e.id}>
                                            {e.lastName} {e.firstName}
                                        </option>
                                    ))
                                )}
                            </select>
                        </div>
                    )}
                </div>
            </PaperCard>

            <div className="cards-grid">
                {            }
                <div className="report-action-card">
                    <h2>Raporty Indywidualne</h2>
                    <p>
                        {isEmployee 
                            ? "Pobierz arkusz zawierający Twoje osiągnięcia." 
                            : `Pobierz arkusz pracownika: ${selectedEmployeeNameClean.replace("_", " ")}.`}
                    </p>
                    <div className="card-buttons-stack">
                        <button 
                            className="btn-action-rep pdf-style" 
                            disabled={reporting || !selectedEmployeeId}
                            onClick={() => downloadReport(`employee/${selectedEmployeeId}/pdf`, `Arkusz_Oceny_${selectedEmployeeNameClean}_${selectedPeriodNameClean}.pdf`)}
                        >
                            {reporting ? "Generowanie PDF..." : "Pobierz Arkusz (PDF)"}
                        </button>
                        <button 
                            className="btn-action-rep excel-style" 
                            disabled={reporting || !selectedEmployeeId}
                            onClick={() => downloadReport(`employee/${selectedEmployeeId}/excel`, `Arkusz_Oceny_${selectedEmployeeNameClean}_${selectedPeriodNameClean}.csv`)}
                        >
                            {reporting ? "Generowanie CSV..." : "Pobierz Arkusz (Excel/CSV)"}
                        </button>
                    </div>
                </div>

                {               }
                {!isEmployee && (
                    <div className="report-action-card admin-card">
                        <h2>Raporty Zbiorcze Komisji</h2>
                        <p>Pobierz raporty zbiorcze.</p>
                        <div className="card-buttons-stack">
                            <button 
                                className="btn-action-rep pdf-dark-style" 
                                disabled={reporting}
                                onClick={() => downloadReport("summary", `Raport_Zbiorczy_${selectedPeriodNameClean}.pdf`)}
                            >
                                {reporting ? "Przetwarzanie pakietu..." : "Pobierz Zestawienie (PDF)"}
                            </button>
                            <button 
                                className="btn-action-rep excel-dark-style" 
                                disabled={reporting}
                                onClick={() => downloadReport("summary/excel", `Raport_Zbiorczy_${selectedPeriodNameClean}.csv`)}
                            >
                                {reporting ? "Przetwarzanie pakietu..." : "Pobierz Zestawienie (Excel/CSV)"}
                            </button>
                        </div>
                    </div>
                )}
            </div>

            <style>{`
                .reports-view-container { padding: 2rem; max-width: 1000px; margin: 0 auto; font-family: 'Inter', sans-serif; }
                .reports-view-header { margin-bottom: 2rem; }
                .reports-back-btn { background: none; border: none; color: #4e73df; cursor: pointer; font-weight: 600; padding: 0; margin-bottom: 0.5rem; font-size: 0.9rem; display: block; }
                .reports-view-header h1 { font-size: 1.8rem; margin: 0; color: #0f172a; font-weight: 800; }
                .reports-subtitle { margin: 4px 0 0 0; color: #64748b; font-size: 0.95rem; }

                /* Style paska filtrów */
                .filter-card-content { display: flex; gap: 30px; flex-wrap: wrap; align-items: center; }
                .filter-box-item { display: flex; flex-direction: column; gap: 6px; flex-grow: 1; min-width: 250px; }
                .filter-box-item label { font-size: 0.8rem; font-weight: 700; color: #475569; text-transform: uppercase; letter-spacing: 0.05em; }
                .select-reports-dropdown { padding: 10px 14px; border: 1px solid #cbd5e1; border-radius: 8px; background-color: #f8fafc; color: #0f172a; font-size: 0.95rem; font-weight: 600; cursor: pointer; outline: none; width: 100%; box-sizing: border-box; }
                .select-reports-dropdown:focus { border-color: #4e73df; box-shadow: 0 0 0 3px rgba(78, 115, 223, 0.15); }
                .select-reports-dropdown.employee-select { border-color: #cbd5e1; background-color: #f0f4ff; color: #1e3a8a; }

                /* Grid z kartami akcji */
                .cards-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(320px, 1fr)); gap: 1.5rem; margin-top: 1.5rem; }
                .report-action-card { background: #ffffff; border: 1px solid #e2e8f0; border-radius: 14px; padding: 2rem; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05); display: flex; flex-direction: column; }
                .report-action-card.admin-card { background: #fafafa; border-color: #cbd5e1; }
                .card-icon { font-size: 2rem; margin-bottom: 1rem; width: 50px; height: 50px; display: flex; align-items: center; justify-content: center; border-radius: 10px; }
                .em-icon { background: #eff6ff; }
                .adm-icon { background: #f0fdf4; }
                .report-action-card h2 { margin: 0 0 0.5rem 0; font-size: 1.3rem; color: #1e293b; font-weight: 700; }
                .report-action-card p { margin: 0 0 2rem 0; font-size: 0.9rem; color: #64748b; line-height: 1.5; flex-grow: 1; min-height: 54px; }
                
                /* Przyciski w kartach */
                .card-buttons-stack { display: flex; flex-direction: column; gap: 10px; }
                .btn-action-rep { border: none; padding: 12px; border-radius: 8px; font-weight: 700; font-size: 0.9rem; cursor: pointer; transition: 0.2s ease; text-align: center; }
                .btn-action-rep:disabled { opacity: 0.4; cursor: not-allowed; background: #e2e8f0 !important; color: #94a3b8 !important; border-color: #cbd5e1 !important; }
                
                .btn-action-rep.pdf-style { background: #fee2e2; color: #991b1b; }
                .btn-action-rep.pdf-style:hover:not(:disabled) { background: #fca5a5; }
                .btn-action-rep.excel-style { background: #dcfce7; color: #166534; }
                .btn-action-rep.excel-style:hover:not(:disabled) { background: #86efac; }
                
                .btn-action-rep.pdf-dark-style { background: #1e1b4b; color: #ffffff; }
                .btn-action-rep.pdf-dark-style:hover:not(:disabled) { background: #312e81; }
                .btn-action-rep.excel-dark-style { background: #022c22; color: #ffffff; }
                .btn-action-rep.excel-dark-style:hover:not(:disabled) { background: #065f46; }

                .loader-reports { padding: 100px; text-align: center; font-size: 1.2rem; color: #64748b; font-weight: 500; }
            `}</style>
        </div>
    );
};

const PaperCard: React.FC<{ children: React.ReactNode }> = ({ children }) => (
    <div style={{
        background: '#ffffff',
        border: '1px solid #e2e8f0',
        padding: '1.5rem',
        borderRadius: '12px',
        boxShadow: '0 1px 3px rgba(0,0,0,0.05)'
    }}>{children}</div>
);

export default ReportsPage;