import networkx as nx
import matplotlib.pyplot as plt
class GraphVisualization:
    def __init__(self):
        self.visual = []
    def visualize(self):
        G = nx.Graph()
        G.add_edges_from(self.visual)
        nx.draw_networkx(G)
        plt.savefig("Graph.png", format="PNG")
        plt.show()

    def addEdgesFromFile(self):
        f = open("nodes.txt", 'r')
        for line in f:
            [e1,e2] = line.split()
            self.visual.append([e1, e2])
            
G = GraphVisualization()
G.addEdgesFromFile()
G.visualize()
