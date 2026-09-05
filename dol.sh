#!/bin/bash

# ============================================================
#  DOL Platform — All-in-One Runner Script
#  Usage:
#    ./dol.sh              → Run full project (Identity API + Gateway)
#    ./dol.sh start        → Start all services in background (daemon)
#    ./dol.sh stop         → Stop all running DOL services
#    ./dol.sh restart      → Clean restart of all services
#    ./dol.sh status       → Check current status of services
#    ./dol.sh identity     → Run Identity API only
#    ./dol.sh gateway      → Run Gateway only
#    ./dol.sh build        → Build all projects without running
# ============================================================

set -e

# ── Colors ───────────────────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m' # No Color

# ── Paths ────────────────────────────────────────────────────
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
IDENTITY_PROJECT="$SCRIPT_DIR/src/Services/Identity/DOL.Identity.API/DOL.Identity.API.csproj"
GATEWAY_PROJECT="$SCRIPT_DIR/src/DOL.Gateway/DOL.Gateway.csproj"
PID_DIR="$SCRIPT_DIR/.dol"

# ── URLs & Ports ─────────────────────────────────────────────
IDENTITY_PORT="5065"
GATEWAY_PORT="5104"
IDENTITY_URL="http://localhost:$IDENTITY_PORT"
GATEWAY_URL="http://localhost:$GATEWAY_PORT"
SWAGGER_URL="$IDENTITY_URL/swagger/index.html"

# ── Helpers ──────────────────────────────────────────────────
banner() {
    echo ""
    echo -e "${CYAN}${BOLD}"
    echo "  ╔══════════════════════════════════════════════════════╗"
    echo "  ║             🚀  DOL AUTOMOTIVE PLATFORM              ║"
    echo "  ║      Multi-Tenant • Concurrency-Safe • Gateway       ║"
    echo "  ╚══════════════════════════════════════════════════════╝"
    echo -e "${NC}"
}

log_info()    { echo -e "  ${BLUE}[INFO]${NC}    $1"; }
log_success() { echo -e "  ${GREEN}[OK]${NC}      $1"; }
log_warn()    { echo -e "  ${YELLOW}[WARN]${NC}    $1"; }
log_error()   { echo -e "  ${RED}[ERROR]${NC}   $1"; }

ensure_pid_dir() {
    mkdir -p "$PID_DIR"
}

save_pid() {
    echo "$2" > "$PID_DIR/$1.pid"
}

get_pid() {
    local pid_file="$PID_DIR/$1.pid"
    if [ -f "$pid_file" ]; then
        cat "$pid_file"
    fi
}

is_running() {
    local pid=$(get_pid "$1")
    if [ -n "$pid" ] && kill -0 "$pid" 2>/dev/null; then
        return 0
    fi
    return 1
}

free_port() {
    local port=$1
    local pids=$(lsof -ti :$port 2>/dev/null || true)
    if [ -n "$pids" ]; then
        log_warn "Port $port is currently occupied by PID(s): $pids. Freeing port..."
        kill -9 $pids 2>/dev/null || true
        sleep 1
    fi
}

check_database() {
    log_info "Checking PostgreSQL connection (localhost:5432)..."
    if nc -z -w 2 localhost 5432 2>/dev/null; then
        log_success "PostgreSQL is reachable on port 5432."
    elif command -v docker >/dev/null 2>&1 && [ -f "$SCRIPT_DIR/docker-compose.yml" ]; then
        log_warn "PostgreSQL not detected. Attempting to start via Docker..."
        docker compose up -d postgres
        sleep 3
        if nc -z -w 2 localhost 5432 2>/dev/null; then
            log_success "PostgreSQL started successfully via Docker."
        else
            log_warn "PostgreSQL container starting; please verify if connection fails."
        fi
    else
        log_warn "PostgreSQL is not responding on port 5432. Ensure PostgreSQL service is active."
    fi
}

cleanup() {
    echo ""
    log_warn "Shutting down all DOL services..."
    stop_services
    echo ""
    log_success "All DOL services stopped cleanly. Goodbye! 👋"
    exit 0
}

# ── Build ────────────────────────────────────────────────────
build_all() {
    log_info "Building DOL solution (.NET 10)..."
    dotnet build "$SCRIPT_DIR/DOL.slnx" --verbosity quiet
    log_success "Build completed successfully!"
}

# ── Start Services ───────────────────────────────────────────
start_identity() {
    if is_running "identity"; then
        log_info "Identity API is already running (PID: $(get_pid identity))."
        return
    fi

    free_port "$IDENTITY_PORT"

    log_info "Starting Identity & Automotive API on ${BOLD}$IDENTITY_URL${NC}..."
    ASPNETCORE_ENVIRONMENT=Development dotnet run --project "$IDENTITY_PROJECT" --no-launch-profile \
        --urls "$IDENTITY_URL" \
        > "$PID_DIR/identity.log" 2>&1 &
    save_pid "identity" $!
    log_success "Identity API launched (PID: $!)."
}

start_gateway() {
    if is_running "gateway"; then
        log_info "YARP Gateway is already running (PID: $(get_pid gateway))."
        return
    fi

    free_port "$GATEWAY_PORT"

    log_info "Starting YARP API Gateway on ${BOLD}$GATEWAY_URL${NC}..."
    ASPNETCORE_ENVIRONMENT=Development dotnet run --project "$GATEWAY_PROJECT" --no-launch-profile \
        --urls "$GATEWAY_URL" \
        > "$PID_DIR/gateway.log" 2>&1 &
    save_pid "gateway" $!
    log_success "YARP Gateway launched (PID: $!)."
}

# ── Stop Services ────────────────────────────────────────────
stop_service() {
    local name=$1
    local pid=$(get_pid "$name")
    if [ -n "$pid" ] && kill -0 "$pid" 2>/dev/null; then
        kill "$pid" 2>/dev/null || true
        wait "$pid" 2>/dev/null || true
        rm -f "$PID_DIR/$name.pid"
        log_success "Stopped $name (PID: $pid)"
    else
        rm -f "$PID_DIR/$name.pid"
    fi
}

stop_services() {
    stop_service "identity"
    stop_service "gateway"
    free_port "$IDENTITY_PORT"
    free_port "$GATEWAY_PORT"
}

# ── Wait for Service ────────────────────────────────────────
wait_for_service() {
    local name=$1
    local url=$2
    local max_attempts=35

    for i in $(seq 1 $max_attempts); do
        if curl -s -o /dev/null -w "" "$url" 2>/dev/null; then
            return 0
        fi
        sleep 1
    done
    return 1
}

# ── Print Status ─────────────────────────────────────────────
print_status() {
    echo ""
    echo -e "  ${BOLD}──────────────────────────────────────────────────────${NC}"
    echo -e "  ${BOLD}  DOL Platform Services are ONLINE & HEALTHY! 🟢      ${NC}"
    echo -e "  ${BOLD}──────────────────────────────────────────────────────${NC}"
    echo -e "  ${GREEN}▸${NC} ${BOLD}Swagger UI${NC}        :  \033[4;34m$SWAGGER_URL\033[0m"
    echo -e "  ${GREEN}▸${NC} ${BOLD}Identity API${NC}      :  $IDENTITY_URL"
    echo -e "  ${GREEN}▸${NC} ${BOLD}Identity Health${NC}   :  $IDENTITY_URL/health"
    echo -e "  ${GREEN}▸${NC} ${BOLD}YARP Gateway${NC}      :  $GATEWAY_URL"
    echo -e "  ${GREEN}▸${NC} ${BOLD}Gateway Health${NC}    :  $GATEWAY_URL/health"
    echo -e "  ${GREEN}▸${NC} ${BOLD}Proxied Swagger${NC}   :  $GATEWAY_URL/swagger/index.html"
    echo -e "  ${BOLD}──────────────────────────────────────────────────────${NC}"
    echo -e "  ${PURPLE}Key Modules Active:${NC}"
    echo -e "   • Multi-Tenant Hierarchy (Company → Country → State → City → Branch)"
    echo -e "   • Concurrency-Safe Vehicle Booking (15-min Reservation Lock + VIN)"
    echo -e "   • FIFO Out-of-Stock Priority Waitlist"
    echo -e "   • Dynamic On-Road Price Quotation (RTO Slabs, Insurance, TCS)"
    echo -e "  ${BOLD}──────────────────────────────────────────────────────${NC}"
}

# ── Logs ─────────────────────────────────────────────────────
tail_logs() {
    echo ""
    echo -e "  ${YELLOW}Streaming logs (Press Ctrl+C to stop all services):${NC}"
    echo ""
    tail -f "$PID_DIR"/*.log 2>/dev/null
}

# ── Check Status ─────────────────────────────────────────────
check_status() {
    echo ""
    echo -e "  ${BOLD}DOL Services Status:${NC}"
    if is_running "identity"; then
        log_success "Identity API is running (PID: $(get_pid identity)) at $IDENTITY_URL"
    else
        log_warn "Identity API is NOT running."
    fi

    if is_running "gateway"; then
        log_success "YARP Gateway is running (PID: $(get_pid gateway)) at $GATEWAY_URL"
    else
        log_warn "YARP Gateway is NOT running."
    fi
    echo ""
}

# ── Main ─────────────────────────────────────────────────────
main() {
    banner
    ensure_pid_dir

    local command="${1:-all}"

    case "$command" in
        build)
            build_all
            ;;
        status)
            check_status
            ;;
        stop)
            stop_services
            log_success "All DOL services stopped."
            ;;
        restart)
            stop_services
            main all
            ;;
        start)
            check_database
            build_all
            start_identity
            log_info "Waiting for Identity API to initialize..."
            if wait_for_service "identity" "$IDENTITY_URL/health"; then
                log_success "Identity API is ready! ✅"
            else
                log_warn "Identity API is starting up. Check $PID_DIR/identity.log"
            fi

            start_gateway
            log_info "Waiting for Gateway to initialize..."
            if wait_for_service "gateway" "$GATEWAY_URL/health"; then
                log_success "YARP Gateway is ready! ✅"
            fi

            print_status
            echo ""
            log_success "Services started in background. Use './dol.sh stop' to shutdown."
            echo ""
            ;;
        identity)
            trap cleanup SIGINT SIGTERM
            check_database
            build_all
            start_identity
            echo ""
            log_info "Waiting for Identity API to initialize..."
            if wait_for_service "identity" "$IDENTITY_URL/health"; then
                log_success "Identity API is ready! ✅"
                echo -e "  ${GREEN}▸${NC} Swagger UI : $SWAGGER_URL"
            else
                log_warn "Identity API may still be starting up. Check logs: $PID_DIR/identity.log"
            fi
            tail_logs
            ;;
        gateway)
            trap cleanup SIGINT SIGTERM
            build_all
            start_gateway
            echo ""
            log_info "Waiting for Gateway to initialize..."
            if wait_for_service "gateway" "$GATEWAY_URL/health"; then
                log_success "Gateway is ready! ✅"
                echo -e "  ${GREEN}▸${NC} Gateway : $GATEWAY_URL"
            fi
            tail_logs
            ;;
        all|run|"")
            trap cleanup SIGINT SIGTERM
            check_database
            build_all
            echo ""

            # Start Identity first, then Gateway
            start_identity

            log_info "Waiting for Identity API to initialize & apply DB migrations..."
            if wait_for_service "identity" "$IDENTITY_URL/health"; then
                log_success "Identity API is ready! ✅"
            else
                log_warn "Identity API may still be starting. Check logs: $PID_DIR/identity.log"
            fi

            start_gateway

            log_info "Waiting for Gateway to initialize..."
            if wait_for_service "gateway" "$GATEWAY_URL/health"; then
                log_success "Gateway is ready! ✅"
            else
                log_warn "Gateway may still be starting. Check logs: $PID_DIR/gateway.log"
            fi

            print_status
            tail_logs
            ;;
        *)
            echo -e "  ${RED}Unknown command: $command${NC}"
            echo ""
            echo "  Usage: ./dol.sh [command]"
            echo ""
            echo "  Commands:"
            echo "    (none)      Run all services in foreground with live logs"
            echo "    start       Start all services in background (daemon mode)"
            echo "    stop        Stop all running services"
            echo "    restart     Restart all services"
            echo "    status      Check status of services"
            echo "    identity    Run Identity API only"
            echo "    gateway     Run Gateway only"
            echo "    build       Build all projects"
            echo ""
            exit 1
            ;;
    esac
}

main "$@"
